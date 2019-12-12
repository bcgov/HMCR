using CsvHelper;
using CsvHelper.Configuration;
using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
using Hmcr.Model.Dtos.WildlifeReport;
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
    public interface IWildlifeReportService
    {
        Task<(decimal SubmissionObjectId, Dictionary<string, List<string>> Errors)> CreateReportAsync(FileUploadDto upload);
    }
    public class WildlifeReportService : IWildlifeReportService
    {
        private IUnitOfWork _unitOfWork;
        private ISubmissionStreamService _streamService;
        private ISubmissionObjectRepository _submissionRepo;
        private IContractTermRepository _contractRepo;
        private ISubmissionStatusRepository _statusRepo;

        public WildlifeReportService(IUnitOfWork unitOfWork,
            ISubmissionStreamService streamService, ISubmissionObjectRepository submissionRepo, 
            IContractTermRepository contractRepo, ISubmissionStatusRepository statusRepo)
        {
            _unitOfWork = unitOfWork;
            _streamService = streamService;
            _submissionRepo = submissionRepo;
            _contractRepo = contractRepo;
            _statusRepo = statusRepo;
        }

        private async Task<(Dictionary<string, List<string>> Errors, SubmissionObjectCreateDto Submission)> ValidateAndParseUploadFileAsync(FileUploadDto upload)
        {
            var errors = new Dictionary<string, List<string>>();

            var reportType = await _streamService.GetSubmissionStreamByTableNameAsync(TableNames.WildlifeReport);
            if (reportType == null)
            {
                throw new Exception($"The submission stream for {TableNames.WildlifeReport} is not defined.");
            }

            var submission = new SubmissionObjectCreateDto();
            submission.FileName = Path.GetFileName(upload.ReportFile.FileName).SanitizeFileName() + ".csv";
            submission.MimeTypeId = 1;
            submission.ServiceAreaNumber = upload.ServiceAreaNumber;
            submission.SubmissionStreamId = reportType.SubmissionStreamId;

            if (!upload.ReportFile.FileName.IsCsvFile())
            {
                errors.AddItem("FileName", $"The file is not a CSV file.");
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
            csv.Configuration.RegisterClassMap<WildlifeRptInitCsvDtoMap>();

            csv.Configuration.TrimOptions = TrimOptions.Trim;
            csv.Configuration.HeaderValidated = (bool valid, string[] column, int row, ReadingContext context) =>
            {
                if (valid) return;

                errors.AddItem($"{column[0]}", $"The header [{column[0].WordToWords()}] is missing.");
            };

            while (csv.Read())
            {
                WildlifeRptInitCsvDto row = null;

                try
                {
                    row = csv.GetRecord<WildlifeRptInitCsvDto>();
                }
                catch (CsvHelperException ex)
                {
                    break;
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();

                submission.SubmissionRows.Add(new SubmissionRowDto
                {
                    RecordNumber = "",
                    RowValue = line,
                    RowHash = line.GetSha256Hash(),
                    RowStatusId = await _statusRepo.GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.Accepted),
                    EndDate = DateTime.Today
                }); ;
            }

            if (errors.Count > 0)
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

            submission.DigitalRepresentation = stream.ToBytes();
            submission.SubmissionStatusId = await _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, RowStatus.Accepted);

            return (errors, submission);
        }

        public async Task<(decimal SubmissionObjectId, Dictionary<string, List<string>> Errors)> CreateReportAsync(FileUploadDto upload)
        {
            var (Errors, Submission) = await ValidateAndLogReportErrorAsync(upload);

            if (Errors.Count > 0)
            {
                Submission.ErrorDetail = GetErrorDetail(Errors);
                Submission.SubmissionRows = new List<SubmissionRowDto>();
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
                var submissionEntity = await _submissionRepo.CreateSubmissionObjectAsync(Submission);
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
