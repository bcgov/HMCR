﻿using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Dtos.SubmissionStream;
using Hmcr.Model.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services.Base
{
    public abstract class ReportServiceBase
    {
        protected IUnitOfWork _unitOfWork;
        protected IFieldValidatorService _validator;
        protected IServiceAreaService _saService;
        private ILogger _logger;
        protected ISubmissionStreamService _streamService;
        protected ISubmissionObjectRepository _submissionRepo;
        protected ISumbissionRowRepository _rowRepo;
        protected IContractTermRepository _contractRepo;
        protected ISubmissionStatusService _statusService;

        protected string TableName;

        /// <summary>
        /// Indicate if CSV row has ID column such as RecordNumber in WorkReport
        /// </summary>
        protected bool HasRowIdentifier; 
        
        /// <summary>
        /// If CSV row has ID column, set the ID column to this property. For example, in the case of the RockfallReport, it's McrrIncidentNumber. 
        /// </summary>
        protected string RecordNumberFieldName;

        /// <summary>
        /// Date for finding the contract term
        /// </summary>
        protected string DateFieldName;

        public ReportServiceBase(IUnitOfWork unitOfWork,
            ISubmissionStreamService streamService, ISubmissionObjectRepository submissionRepo, ISumbissionRowRepository rowRepo,
            IContractTermRepository contractRepo, ISubmissionStatusService statusService, IFieldValidatorService validator, 
            IServiceAreaService saService, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _streamService = streamService;
            _submissionRepo = submissionRepo;
            _rowRepo = rowRepo;
            _contractRepo = contractRepo;
            _statusService = statusService;
            _validator = validator;
            _saService = saService;
            _logger = logger;
        }
        public async Task<(Dictionary<string, List<string>> errors, List<string> resubmittedRecordNumbers)> CheckResubmitAsync(FileUploadDto upload)
        {
            if (!HasRowIdentifier)
                return (new Dictionary<string, List<string>>(), null);

            var instance = Guid.NewGuid();

            _logger.LogInformation($"[Api] {instance} Checking resubmit started.");

            var (errors, submission) = await ValidateAndLogReportErrorAsync(upload);

            _logger.LogInformation($"[Api] {instance} Checking resubmit finished.");

            //if there are any errors, just log it and report them to the user.
            if (errors.Count > 0)
                return (errors, null);

            var resubmittedRecordNumbers = submission.SubmissionRows.Where(x => x.IsResubmitted).Select(x => x.RecordNumber).ToList();

            return (errors, resubmittedRecordNumbers);
        }

        public async Task<(decimal submissionObjectId, Dictionary<string, List<string>> errors)> CreateReportAsync(FileUploadDto upload)
        {
            var instance = Guid.NewGuid();

            _logger.LogInformation($"[Api] {instance} Creating report started.");

            var (errors, submission) = await ValidateAndLogReportErrorAsync(upload);

            if (errors.Count > 0)
            {
                _logger.LogInformation($"[Api] {instance} Creating report finished with errors.");
                return (0, errors);
            }

            var submissionEntity = await _submissionRepo.CreateSubmissionObjectAsync(submission);
            _unitOfWork.Commit();

            _logger.LogInformation($"[Api] {instance} Creating report finished - submission #: {submissionEntity.SubmissionObjectId}");

            return (submissionEntity.SubmissionObjectId, errors);
        }

        private async Task<(Dictionary<string, List<string>> errors, SubmissionObjectCreateDto submission)> ValidateAndLogReportErrorAsync(FileUploadDto upload)
        {
            var (errors, submission) = await ValidateAndParseUploadFileAsync(upload);

            if (errors.Count > 0) //save only when there are any errors
            {
                submission.ErrorDetail = errors.GetErrorDetail();
                submission.SubmissionRows = new List<SubmissionRowDto>();
                submission.DigitalRepresentation = null;
                await _submissionRepo.CreateSubmissionObjectAsync(submission);
                _unitOfWork.Commit();
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
            submission.SubmissionStatusId = _statusService.FileError;
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

            if (submission.FileName.Length > 100)
            {
                submission.FileName = submission.FileName.Substring(0, 90) + ".csv";
                errors.AddItem("File", "the filename needs to be shorter than 100 characters");
                submission.SubmissionStatusId = _statusService.FileError;
                return (errors, submission);
            }

            using var stream = upload.ReportFile.OpenReadStream();
            using TextReader textReader = new StreamReader(stream, Encoding.UTF8);

            if (CheckFileContents(submission, reportType, stream, errors))
            {
                return (errors, submission);
            }

            if (await _submissionRepo.IsDuplicateFileAsync(submission))
            {
                errors.AddItem("File", "Duplicate file exists");
                submission.SubmissionStatusId = _statusService.FileDuplicate;
                return (errors, submission);
            }

            if (!await ParseRowsAsync(submission, textReader, errors))
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

            var contract = await _contractRepo.GetContractTerm(submission.ServiceAreaNumber, submission.SubmissionRows.Max(x => x.EndDate));

            if (contract == null)
            {
                submission.FileHash = null; //it's an error outside of the file; user can submit the same file again and the system should be able to accept it.
                errors.AddItem(DateFieldName, $"Contract term not found for the submission. The contract may have expired, or a row shows an end date beyond the contract's expiration.");
                return (errors, submission);
            }


            submission.PartyId = contract.PartyId;
            submission.ContractTermId = contract.ContractTermId;

            await MarkDuplicateRowAsync(submission);

            //set IsResubmitted
            await foreach (var resubmittedRecordNumber in _rowRepo.UpdateIsResubmitAsync(submission.SubmissionStreamId, (decimal)submission.ContractTermId, submission.SubmissionRows)) { }

            submission.SubmissionStatusId = _statusService.FileReceived;

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
                await foreach (var row in _rowRepo.FindDuplicateFromLatestRecordsAsync(submission.SubmissionStreamId, (decimal)submission.ContractTermId, submission.SubmissionRows))
                {
                    row.RowStatusId = _statusService.RowDuplicate;
                }
            }
            else
            {
                await foreach (var row in _rowRepo.FindDuplicateFromAllRecordsAsync(submission.SubmissionStreamId, submission.SubmissionRows))
                {
                    row.RowStatusId = _statusService.RowDuplicate;
                }
            }
        }

        private bool CheckFileContents(SubmissionObjectCreateDto submission, SubmissionStreamDto reportType, Stream stream, Dictionary<string, List<string>> errors)
        {
            var bytes = stream.ToBytes();
            var size = bytes.Length;

            var maxSize = reportType.FileSizeLimit ?? Constants.MaxFileSize;
            if (size > maxSize)
            {
                errors.AddItem("FileSize", $"The file size exceeds the maximum size {maxSize / 1024 / 1024}MB.");
                return true;
            }

            submission.FileHash = bytes.GetSha256Hash();
            submission.DigitalRepresentation = bytes;

            return false;
        }
        protected virtual Task<bool> ParseRowsAsync(SubmissionObjectCreateDto submission, TextReader textReader, Dictionary<string, List<string>> errors)
        {
            throw new NotImplementedException();
        }

        protected void Validate<T>(IEnumerable<T> rows, string entityName, Dictionary<string, List<string>> errors) where T: IRptInitCsvDto
        {
            foreach(var row in rows)
            {                
                _validator.Validate<T>(entityName, row, errors, row.RowNum);

                if (errors.Count > 10)
                {
                    errors.AddItem("File", "File validation stopped after 10 errors were found. There can be more errors.");
                    break;
                }
            }
        }

        protected static bool CheckCommonMandatoryFields(string[] headers, string[] mandatoryFields, Dictionary<string, List<string>> errors)
        {
            headers = CsvUtils.GetLowercaseFieldsFromCsvHeaders(headers);

            foreach (var field in mandatoryFields)
            {
                if (!headers.Any(x => x == field.ToLowerInvariant()))
                    errors.AddItem("File", $"Header [{field.WordToWords()}] is missing");
            }

            if (errors.Count > 0)
                errors.AddItem("File", "Please ensure the file headers are correct and correct file type is chosen");

            return errors.Count == 0;
        }

        protected static string[] ConvertServiceAreaToStrings(decimal serviceArea)
        {
            return new string[] { ((long)serviceArea).ToString(), string.Format("{0, 2:00}", serviceArea) };
        }
    }
}
