using CsvHelper;
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
        protected bool CheckDuplicate;
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
        public async Task<(Dictionary<string, List<string>> errors, List<string> duplicateRecordNumbers)> CheckDuplicatesAsync(FileUploadDto upload)
        {
            var recordNumbers = new List<string>();

            if (!CheckDuplicate)
                return (new Dictionary<string, List<string>>(), recordNumbers);

            var (errors, submission) = await ValidateAndLogReportErrorAsync(upload);

            if (errors.Count > 0)
                return (errors, recordNumbers);

            await foreach (var recordNumber in _rowRepo.FindDuplicateRowsToOverwriteAsync(submission.SubmissionStreamId, (decimal)submission.PartyId, submission.SubmissionRows))
            {
                recordNumbers.Add(recordNumber);
            }

            return (errors, recordNumbers);
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

            if (CheckDuplicate && HasDuplicateRecordNumberInFile(submission.SubmissionRows, errors))
            {
                return (errors, submission);
            }

            var partyId = await _contractRepo.GetContractPartyId(submission.ServiceAreaNumber, submission.SubmissionRows.Max(x => x.EndDate));

            if (partyId == 0)
            {
                submission.FileHash = null; //it's an error outside of the file; user can submit the same file again and the system should be able to accept it.
                errors.AddItem("EndDate", $"Cannot find the contract term for this file");
                return (errors, submission);
            }

            submission.PartyId = partyId;

            if (CheckDuplicate)
                await MarkDuplicateRowAsync(submission);

            submission.DigitalRepresentation = stream.ToBytes();
            submission.SubmissionStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.File, FileStatus.FileReceived);

            return (errors, submission);
        }

        private async Task MarkDuplicateRowAsync(SubmissionObjectCreateDto submission)
        {
            await foreach (var row in _rowRepo.FindDuplicateRowsAsync(submission.SubmissionStreamId, (decimal)submission.PartyId, submission.SubmissionRows))
            {
                row.RowStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.Row, RowStatus.DuplicateRow);
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

        private async Task<(Dictionary<string, List<string>> errors, SubmissionObjectCreateDto submission)> ValidateAndLogReportErrorAsync(FileUploadDto upload)
        {
            var (errors, submission) = await ValidateAndParseUploadFileAsync(upload);

            if (errors.Count > 0)
            {
                submission.ErrorDetail = errors.GetErrorDetail();
                submission.SubmissionRows = new List<SubmissionRowDto>();
                await _submissionRepo.CreateSubmissionObjectAsync(submission);
                await _unitOfWork.CommitAsync();
            }

            return (errors, submission);
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
    }
}
