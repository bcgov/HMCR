using CsvHelper;
using CsvHelper.Configuration;
using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Model;
using Hmcr.Model.Dtos.RockfallReport;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.SubmissionRow;
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
    public interface IRockfallReportService
    {
        Task<(Dictionary<string, List<string>> Errors, List<string> DuplicateRecordNumbers)> CheckDuplicatesAsync(FileUploadDto upload);
        Task<(decimal SubmissionObjectId, Dictionary<string, List<string>> Errors)> CreateRockfallReportAsync(FileUploadDto upload);

    }
    public class RockfallReportService : IRockfallReportService
    {
        private IUnitOfWork _unitOfWork;
        private HmcrCurrentUser _currentUser;
        private ISubmissionStreamService _streamService;
        private ISubmissionObjectRepository _submissionRepo;
        private ISumbissionRowRepository _rowRepo;
        private IContractTermRepository _contractRepo;
        private ISubmissionStatusRepository _statusRepo;

        public RockfallReportService(IUnitOfWork unitOfWork, HmcrCurrentUser currentUser, 
            ISubmissionStreamService streamService, ISubmissionObjectRepository submissionRepo, ISumbissionRowRepository rowRepo,
            IContractTermRepository contractRepo, ISubmissionStatusRepository statusRepo)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
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

            var (Errors, Submission) = await ValidateAndParseUploadFileAsync(upload);

            if (Errors.Count > 0)
                return (Errors, recordNumbers);

            await foreach (var recordNumber in _rowRepo.FindDuplicateRowsToOverwriteAsync(Submission.SubmissionStreamId, Submission.SubmissionRows))
            {
                recordNumbers.Add(recordNumber);
            }

            return (errors, recordNumbers);
        }

        private async Task<(Dictionary<string, List<string>> Errors, SubmissionObjectCreateDto Submission)> ValidateAndParseUploadFileAsync(FileUploadDto upload)
        {
            var errors = new Dictionary<string, List<string>>();

            var reportType = await _streamService.GetSubmissionStreamByTableNameAsync(TableNames.RockfallReport);
            if (reportType == null)
            {
                throw new Exception($"The submission stream for {TableNames.RockfallReport} is not defined.");
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

            var serviceArea = _currentUser.UserInfo.ServiceAreas.FirstOrDefault(x => x.ServiceAreaNumber == upload.ServiceAreaNumber);
            if (serviceArea == null)
            {
                errors.AddItem("SerivceArea", $"The user has no access to the service area {upload.ServiceAreaNumber}.");
                return (errors, submission);
            }

            using var stream = upload.ReportFile.OpenReadStream();
            using var text = new StreamReader(stream, Encoding.UTF8);
            using var csv = new CsvReader(text);

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
            csv.Configuration.RegisterClassMap<RockfallRptInitCsvDtoMap>();

            csv.Configuration.TrimOptions = TrimOptions.Trim;
            csv.Configuration.HeaderValidated = (bool valid, string[] column, int row, ReadingContext context) =>
            {
                if (valid) return;

                errors.AddItem($"{column[0]}", $"The header [{column[0].WordToWords()}] is missing.");
            };

            while (csv.Read())
            {
                RockfallRptInitCsvDto row = null;

                try
                {
                    row = csv.GetRecord<RockfallRptInitCsvDto>();
                }
                catch (CsvHelperException ex)
                {
                    break;
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();

                submission.SubmissionRows.Add(new SubmissionRowDto
                {
                    RecordNumber = row.MajorIncidentNumber,
                    RowValue = line,
                    RowHash = line.GetSha256Hash(),
                    RowStatusId = await _statusRepo.GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.Accepted),
                    EndDate = (DateTime)row.ReportDate
                });
            }

            if (errors.Count > 0)
            {
                return (errors, submission);
            }

            if (!await _contractRepo.HasContractTermAsync(upload.ServiceAreaNumber, submission.SubmissionRows.Max(x => x.EndDate)))
            {
                errors.AddItem("EndDate", $"Cannot find the contract term for this file");
                return (errors, submission);
            }

            await foreach (var row in _rowRepo.FindDuplicateRowsAsync(submission.SubmissionStreamId, submission.SubmissionRows))
            {
                row.RowStatusId = await _statusRepo.GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.Duplicate);
            }

            if (errors.Count == 0)
            {
                submission.DigitalRepresentation = stream.ToBytes();
                submission.SubmissionStatusId = await _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, RowStatus.Accepted);
            }

            return (errors, submission);
        }

        public async Task<(decimal SubmissionObjectId, Dictionary<string, List<string>> Errors)> CreateRockfallReportAsync(FileUploadDto upload)
        {
            var (Errors, Submission) = await ValidateAndParseUploadFileAsync(upload);

            if (Errors.Count > 0)
                return (0, Errors);

            var submissionEntity = await _submissionRepo.CreateSubmissionObjectAsync(Submission);
            await _unitOfWork.CommitAsync();

            return (submissionEntity.SubmissionObjectId, null);
        }

    }
}
