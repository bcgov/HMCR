using CsvHelper;
using CsvHelper.Configuration;
using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
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

namespace Hmcr.Domain.Services
{
    public interface IWorkReportService
    {
        Task<(Dictionary<string, List<string>> Errors, List<string> DuplicateRecordNumbers)> CheckDuplicatesAsync(FileUploadDto upload);
        Task<(decimal SubmissionObjectId, Dictionary<string, List<string>> Errors)> CreateReportAsync(FileUploadDto upload);
    }
    public class WorkReportService : IWorkReportService
    {
        private IUnitOfWork _unitOfWork;
        private IFieldValidatorService _validator;
        private ISubmissionStreamService _streamService;
        private ISubmissionObjectRepository _submissionRepo;
        private ISumbissionRowRepository _rowRepo;
        private IContractTermRepository _contractRepo;
        private ISubmissionStatusRepository _statusRepo;

        public WorkReportService(IUnitOfWork unitOfWork, IFieldValidatorService validator, 
            ISubmissionStreamService streamService, ISubmissionObjectRepository submissionRepo, ISumbissionRowRepository rowRepo, 
            IContractTermRepository contractRepo, ISubmissionStatusRepository statusRepo)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
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
            foreach(var row in rows)
            {
                if (rows.Count(x => x.RecordNumber == row.RecordNumber) > 1)
                {
                    errors.AddItem("RecordNumber", $"The file contains multiple rows with the same record number {row.RecordNumber}.");
                    return true;
                }
            }

            return false;
        }

        private async Task<(Dictionary<string, List<string>> Errors, SubmissionObjectCreateDto Submission)> ValidateAndParseUploadFileAsync(FileUploadDto upload)
        {
            var errors = new Dictionary<string, List<string>>();

            var reportType = await _streamService.GetSubmissionStreamByTableNameAsync(TableNames.WorkReport);
            if (reportType == null)
            {
                throw new Exception($"The submission stream for {TableNames.WorkReport} is not defined.");
            }

            var submission = new SubmissionObjectCreateDto();
            submission.FileName = Path.GetFileName(upload.ReportFile.FileName).SanitizeFileName() + ".csv";
            submission.MimeTypeId = 1;
            submission.ServiceAreaNumber = upload.ServiceAreaNumber;
            submission.SubmissionStreamId = reportType.SubmissionStreamId;

            if (!upload.ReportFile.FileName.IsCsvFile())
            {
                errors.AddItem("FileName", "The file is not a CSV file.");
                return (errors, submission);
            }

            using var stream = upload.ReportFile.OpenReadStream();
            using var streamReader = new StreamReader(stream, Encoding.UTF8);

            var text = streamReader.ReadToEnd();
            submission.FileHash = text.GetSha256Hash();
            if (await _submissionRepo.IsDuplicateFileAsync(submission))
            {
                errors.AddItem("File", "Duplicate file exists");
                return (errors, submission);
            }

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            //file size
            var size = stream.Length;
            var maxSize = reportType.FileSizeLimit ?? Constants.MaxFileSize;
            if (size > maxSize)
            {
                errors.AddItem("FileSize", $"The file size exceeds the maximum size {maxSize / 1024 / 1024}MB.");
                return (errors, submission);
            }

            csv.Configuration.PrepareHeaderForMatch = (string header, int index) => Regex.Replace(header.ToLower(), @"\s", string.Empty);
            csv.Configuration.CultureInfo = CultureInfo.GetCultureInfo("en-CA");
            csv.Configuration.RegisterClassMap<WorkRptInitCsvDtoMap>();

            csv.Configuration.TrimOptions = TrimOptions.Trim;
            csv.Configuration.HeaderValidated = (bool valid, string[] column, int row, ReadingContext context) =>
            {
                if (valid) return;

                errors.AddItem($"{column[0]}", $"The header [{column[0].WordToWords()}] is missing.");
            };

            while (csv.Read())
            {
                WorkRptInitCsvDto row = null;

                try
                {
                    row = csv.GetRecord<WorkRptInitCsvDto>();
                }
                catch (CsvHelperException)
                {
                    break;
                }

                if (row.ServiceArea != upload.ServiceAreaNumber.ToString())
                {
                    errors.AddItem("ServiceArea", $"The file contains service area which is not {upload.ServiceAreaNumber}.");
                    return (errors, submission);
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();

                submission.SubmissionRows.Add(new SubmissionRowDto
                {
                    RecordNumber = row.RecordNumber,
                    RowValue = line,
                    RowHash = line.GetSha256Hash(),
                    RowStatusId = await _statusRepo.GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.Accepted),
                    EndDate = row.EndDate
                });
            }

            if (errors.Count > 0)
            {
                return (errors, submission);
            }

            if (HasDuplicateRecordNumberInFile(submission.SubmissionRows, errors))
            {
                return (errors, submission);
            }

            var partyId = await _contractRepo.GetContractPartyId(upload.ServiceAreaNumber, submission.SubmissionRows.Max(x => x.EndDate));

            if (partyId == 0)
            {
                errors.AddItem("EndDate", $"Cannot find the contract term for this file");
                return (errors, submission);
            }

            submission.PartyId = partyId;

            await foreach (var row in _rowRepo.FindDuplicateRowsAsync(submission.SubmissionStreamId, submission.SubmissionRows))
            {
                row.RowStatusId = await _statusRepo.GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.Duplicate);
            }

            submission.DigitalRepresentation = stream.ToBytes();
            submission.SubmissionStatusId = await _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, RowStatus.Accepted);

            return (errors, submission); 
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

        public async Task<(Dictionary<string, List<string>> Errors, SubmissionObjectCreateDto Submission)> ValidateAndLogReportErrorAsync(FileUploadDto upload)
        {
            var (Errors, Submission) = await ValidateAndParseUploadFileAsync(upload);

            if (Errors.Count > 0)
            {
                Submission.ErrorDetail = GetErrorDetail(Errors);
                Submission.SubmissionRows = new List<SubmissionRowDto>();
                Submission.SubmissionStatusId = await _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, FileStatus.Error);
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
    }
}
