﻿using CsvHelper;
using CsvHelper.Configuration;
using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Dtos.SubmissionStream;
using Hmcr.Model.Dtos.WorkReport;
using Hmcr.Model.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services.Base
{
    public abstract class ReportServiceBase
    {
        protected IUnitOfWork _unitOfWork;
        protected IFieldValidatorService _validator;
        protected ISubmissionStreamService _streamService;
        protected ISubmissionObjectRepository _submissionRepo;
        protected ISumbissionRowRepository _rowRepo;
        protected IContractTermRepository _contractRepo;
        protected ISubmissionStatusRepository _statusRepo;

        protected string TableName;

        /// <summary>
        /// Indicate if CSV row has ID column such as RecordNumber in WorkReport
        /// </summary>
        protected bool HasRowIdentifier; 
        
        /// <summary>
        /// If CSV row has ID column, set the ID column to this property. For example, in the case of the RockfallReport, it's McrrIncidentNumber. 
        /// </summary>
        protected string RecordNumberFieldName;

        public ReportServiceBase(IUnitOfWork unitOfWork,
            ISubmissionStreamService streamService, ISubmissionObjectRepository submissionRepo, ISumbissionRowRepository rowRepo,
            IContractTermRepository contractRepo, ISubmissionStatusRepository statusRepo)
        {
            _unitOfWork = unitOfWork;
            _streamService = streamService;
            _submissionRepo = submissionRepo;
            _rowRepo = rowRepo;
            _contractRepo = contractRepo;
            _statusRepo = statusRepo;
        }
        public async Task<(Dictionary<string, List<string>> errors, List<string> resubmittedRecordNumbers)> CheckResubmitAsync(FileUploadDto upload)
        {
            if (!HasRowIdentifier)
                return (new Dictionary<string, List<string>>(), null);

            var (errors, submission) = await ValidateAndLogReportErrorAsync(upload);

            //if there are any errors, just log it and report them to the user.
            if (errors.Count > 0)
                return (errors, null);

            var resubmittedRecordNumbers = submission.SubmissionRows.Where(x => x.IsResubmitted == "Y").Select(x => x.RecordNumber).ToList();

            return (errors, resubmittedRecordNumbers);
        }

        public async Task<(decimal submissionObjectId, Dictionary<string, List<string>> errors)> CreateReportAsync(FileUploadDto upload)
        {
            var (errors, submission) = await ValidateAndLogReportErrorAsync(upload);

            if (errors.Count > 0)
            {
                return (0, errors);
            }

            var submissionEntity = await _submissionRepo.CreateSubmissionObjectAsync(submission);
            await _unitOfWork.CommitAsync();

            return (submissionEntity.SubmissionObjectId, errors);
        }

        private async Task<(Dictionary<string, List<string>> errors, SubmissionObjectCreateDto submission)> ValidateAndLogReportErrorAsync(FileUploadDto upload)
        {
            var (errors, submission) = await ValidateAndParseUploadFileAsync(upload);

            if (errors.Count > 0) //save only when there are any errors
            {
                submission.ErrorDetail = errors.GetErrorDetail();
                submission.SubmissionRows = new List<SubmissionRowDto>();
                await _submissionRepo.CreateSubmissionObjectAsync(submission);
                await _unitOfWork.CommitAsync();
            }

            return (errors, submission);
        }

        private async Task<(Dictionary<string, List<string>> errors, SubmissionObjectCreateDto submission)> ValidateAndParseUploadFileAsync(FileUploadDto upload)
        {
            var errors = new Dictionary<string, List<string>>();

            var reportType = await _streamService.GetSubmissionStreamByTableNameAsync(TableName);
            if (reportType == null)
            {
                throw new Exception($"The submission stream for {TableName} is not defined.");
            }

            var submission = new SubmissionObjectCreateDto();
            submission.MimeTypeId = 1;
            submission.ServiceAreaNumber = upload.ServiceAreaNumber;
            submission.SubmissionStreamId = reportType.SubmissionStreamId;
            submission.SubmissionStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.File, FileStatus.FileError);
            submission.FileName = "";

            if (upload.ReportFile == null)
            {
                errors.AddItem("File", $"The file is null or empty.");
                return (errors, submission);
            }

            if (!upload.ReportFile.FileName.IsCsvFile())
            {
                errors.AddItem("FileName", "The file is not a CSV file.");
                return (errors, submission);
            }

            submission.FileName = Path.GetFileName(upload.ReportFile.FileName).SanitizeFileName() + ".csv";

            using var stream = upload.ReportFile.OpenReadStream();
            using var streamReader = new StreamReader(stream, Encoding.UTF8);

            var text = streamReader.ReadToEnd();
            submission.FileHash = text.GetSha256Hash();
            if (await _submissionRepo.IsDuplicateFileAsync(submission))
            {
                errors.AddItem("File", "Duplicate file exists");
                submission.SubmissionStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.File, FileStatus.DuplicateSubmission);
                return (errors, submission);
            }

            if (IsInvalidFileSize(submission, reportType, text, errors))
            {
                return (errors, submission);
            }

            if (!await ParseRowsAsync(submission, text, errors))
            {
                return (errors, submission);
            }

            if (submission.SubmissionRows.Count == 0)
            {
                errors.AddItem("File", "The file contains no rows");
                return (errors, submission);
            }

            if (HasDuplicateInFile(submission.SubmissionRows, errors))
                return (errors, submission);

            var partyId = await _contractRepo.GetContractPartyId(submission.ServiceAreaNumber, submission.SubmissionRows.Max(x => x.EndDate));

            if (partyId == 0)
            {
                submission.FileHash = null; //it's an error outside of the file; user can submit the same file again and the system should be able to accept it.
                errors.AddItem("EndDate", $"Cannot find the contract term for this file");
                return (errors, submission);
            }

            submission.PartyId = partyId;

            await MarkDuplicateRowAsync(submission);

            //set IsResubmitted
            await foreach (var resubmittedRecordNumber in _rowRepo.UpdateIsResubmitAsync(submission.SubmissionStreamId, (decimal)submission.PartyId, submission.SubmissionRows)) { }

            submission.DigitalRepresentation = stream.ToBytes();
            submission.SubmissionStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.File, FileStatus.FileReceived);

            return (errors, submission);
        }

        private bool HasDuplicateInFile(IEnumerable<SubmissionRowDto> rows, Dictionary<string, List<string>> errors)
        {
            return HasRowIdentifier ? HasDuplicateRecordNumberInFile(rows, errors) : HasIdenticalRowInFile(rows, errors);
        }

        private bool HasDuplicateRecordNumberInFile(IEnumerable<SubmissionRowDto> rows, Dictionary<string, List<string>> errors)
        {
            foreach (var row in rows)
            {
                if (rows.Count(x => x.RecordNumber == row.RecordNumber) > 1)
                {
                    errors.AddItem($"{RecordNumberFieldName}", $"Submission has multiple records with the same {RecordNumberFieldName.WordToWords()} {row.RecordNumber}.");
                    return true;
                }
            }

            return false;
        }

        private bool HasIdenticalRowInFile(IEnumerable<SubmissionRowDto> rows, Dictionary<string, List<string>> errors)
        {
            var i = 0;
            foreach (var row in rows)
            {
                i++;

                if (rows.Count(x => x.RowHash == row.RowHash) > 1)
                {
                    errors.AddItem($"File", $"The row #{i} has identical row(s) in the file.");
                    return true;
                }
            }

            return false;
        }

        private async Task MarkDuplicateRowAsync(SubmissionObjectCreateDto submission)
        {
            if (HasRowIdentifier)
            {
                await foreach (var row in _rowRepo.FindDuplicateFromLatestRecordsAsync(submission.SubmissionStreamId, (decimal)submission.PartyId, submission.SubmissionRows))
                {
                    row.RowStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.Row, RowStatus.DuplicateRow);
                }
            }
            else
            {
                await foreach (var row in _rowRepo.FindDuplicateFromAllRecordsAsync(submission.SubmissionStreamId, submission.SubmissionRows))
                {
                    row.RowStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.Row, RowStatus.DuplicateRow);
                }
            }
        }

        private bool IsInvalidFileSize(SubmissionObjectCreateDto submission, SubmissionStreamDto reportType, string text, Dictionary<string, List<string>> errors)
        {
            var size = text.Length;
            var maxSize = reportType.FileSizeLimit ?? Constants.MaxFileSize;
            if (size > maxSize)
            {
                errors.AddItem("FileSize", $"The file size exceeds the maximum size {maxSize / 1024 / 1024}MB.");
                return true;
            }

            return false;
        }
        protected virtual Task<bool> ParseRowsAsync(SubmissionObjectCreateDto submission, string text, Dictionary<string, List<string>> errors)
        {
            throw new NotImplementedException();
        }
    }
}