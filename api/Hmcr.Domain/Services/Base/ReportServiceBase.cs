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
        public async Task<(Dictionary<string, List<string>> Errors, List<string> DuplicateRecordNumbers)> CheckDuplicatesAsync(FileUploadDto upload)
        {
            var errors = new Dictionary<string, List<string>>();
            var recordNumbers = new List<string>();

            if (!CheckDuplicate)
                return (errors, recordNumbers);

            var (Errors, Submission) = await ValidateAndLogReportErrorAsync(upload);

            if (Errors.Count > 0)
                return (Errors, recordNumbers);

            await foreach (var recordNumber in _rowRepo.FindDuplicateRowsToOverwriteAsync(Submission.SubmissionStreamId, Submission.PartyId, Submission.SubmissionRows))
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

        private async Task<(Dictionary<string, List<string>> Errors, SubmissionObjectCreateDto Submission)> ValidateAndParseUploadFileAsync(FileUploadDto upload)
        {
            var errors = new Dictionary<string, List<string>>();

            var reportType = await _streamService.GetSubmissionStreamByTableNameAsync(TableName);
            if (reportType == null)
            {
                throw new Exception($"The submission stream for {TableNames.WorkReport} is not defined.");
            }

            var submission = new SubmissionObjectCreateDto();
            submission.MimeTypeId = 1;
            submission.ServiceAreaNumber = upload.ServiceAreaNumber;
            submission.SubmissionStreamId = reportType.SubmissionStreamId;
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

            if (CheckDuplicate && HasDuplicateRecordNumberInFile(submission.SubmissionRows, errors))
            {
                return (errors, submission);
            }

            var partyId = await _contractRepo.GetContractPartyId(submission.ServiceAreaNumber, submission.SubmissionRows.Max(x => x.EndDate));

            if (partyId == 0)
            {
                errors.AddItem("EndDate", $"Cannot find the contract term for this file");
                return (errors, submission);
            }

            submission.PartyId = partyId;

            if (CheckDuplicate)
                await MarkDuplicateRowAsync(submission);

            submission.DigitalRepresentation = stream.ToBytes();
            submission.SubmissionStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.File, RowStatus.Accepted);

            return (errors, submission);
        }

        private async Task MarkDuplicateRowAsync(SubmissionObjectCreateDto submission)
        {
            await foreach (var row in _rowRepo.FindDuplicateRowsAsync(submission.SubmissionStreamId, submission.SubmissionRows))
            {
                row.RowStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.Row, RowStatus.Duplicate);
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

        protected static void ConfigCsvHelper(Dictionary<string, List<string>> errors, CsvReader csv, bool checkHeader = true)
        {
            csv.Configuration.PrepareHeaderForMatch = (string header, int index) => Regex.Replace(header.ToLower(), @"\s", string.Empty);
            csv.Configuration.CultureInfo = CultureInfo.GetCultureInfo("en-CA");

            csv.Configuration.TrimOptions = TrimOptions.Trim;

            if (checkHeader)
            {
                csv.Configuration.HeaderValidated = (bool valid, string[] column, int row, ReadingContext context) =>
                {
                    if (valid) return;

                    errors.AddItem($"{column[0]}", $"The header [{column[0].WordToWords()}] is missing.");
                };
            }
            else
            {
                csv.Configuration.MissingFieldFound = null;
                csv.Configuration.HeaderValidated = null;
            }
        }

        private async Task<(Dictionary<string, List<string>> Errors, SubmissionObjectCreateDto Submission)> ValidateAndLogReportErrorAsync(FileUploadDto upload)
        {
            var (Errors, Submission) = await ValidateAndParseUploadFileAsync(upload);

            if (Errors.Count > 0)
            {
                Submission.ErrorDetail = GetErrorDetail(Errors);
                Submission.SubmissionRows = new List<SubmissionRowDto>();
                Submission.SubmissionStatusId = await _statusRepo.GetStatusIdByTypeAndCodeAsync(StatusType.File, FileStatus.Error);
                await _submissionRepo.CreateSubmissionObjectAsync(Submission);
                await _unitOfWork.CommitAsync();
            }

            return (Errors, Submission);
        }

        private string GetErrorDetail(Dictionary<string, List<string>> errors)
        {
            var errorDetail = new StringBuilder();
            foreach (var (error, detail) in errors.SelectMany(error => error.Value.Select(detail => (error, detail))))
            {
                errorDetail.AppendLine($"{error.Key}: {detail}");
            }

            return errorDetail.ToString();
        }

        public async Task<(decimal SubmissionObjectId, Dictionary<string, List<string>> Errors)> CreateReportAsync(FileUploadDto upload)
        {
            var (Errors, Submission) = await ValidateAndLogReportErrorAsync(upload);

            if (Errors.Count > 0)
            {
                return (0, Errors);
            }

            var submissionEntity = await _submissionRepo.CreateSubmissionObjectAsync(Submission);
            await _unitOfWork.CommitAsync();

            return (submissionEntity.SubmissionObjectId, Errors);
        }
    }
}
