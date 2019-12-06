using CsvHelper;
using CsvHelper.Configuration;
using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Model;
using Hmcr.Model.Dtos.ServiceArea;
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

namespace Hmcr.Domain.Services
{
    public interface IWorkReportService
    {
        Task<(Dictionary<string, List<string>> Errors, List<string> DuplicateRecordNumbers)> CheckDuplicatesAsync(WorkRptUploadDto upload);
        Task<(decimal SubmissionObjectId, Dictionary<string, List<string>> Errors)> CreateWorkReportAsync(WorkRptUploadDto upload);
    }
    public class WorkReportService : IWorkReportService
    {
        private IUnitOfWork _unitOfWork;
        private HmcrCurrentUser _currentUser;
        private IFieldValidatorService _validator;
        private ISubmissionStreamService _streamService;
        private ISubmissionObjectRepository _submissionRepo;
        private ISumbissionRowRepository _rowRepo;

        public WorkReportService(IUnitOfWork unitOfWork, HmcrCurrentUser currentUser, IFieldValidatorService validator, 
            ISubmissionStreamService streamService, ISubmissionObjectRepository submissionRepo, ISumbissionRowRepository rowRepo)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _validator = validator;
            _streamService = streamService;
            _submissionRepo = submissionRepo;
            _rowRepo = rowRepo;
        }
        public async Task<(Dictionary<string, List<string>> Errors, List<string> DuplicateRecordNumbers)> CheckDuplicatesAsync(WorkRptUploadDto upload)
        {
            var errors = new Dictionary<string, List<string>>();
            var dupRows = new List<string>();

            var (Errors, Submission) = await ValidateAndParseUploadFileAsync(upload);

            if (Errors.Count > 0)
                return (Errors, dupRows);

            //todo: find duplicate rows to overwrite

            return (errors, dupRows);
        }

        private static List<string> GetLines(WorkRptUploadDto upload, Stream stream)
        {
            using var text = new StreamReader(stream, Encoding.UTF8);

            var lines = new List<string>();
            while (!text.EndOfStream)
            {
                lines.Add(text.ReadLine());
            }

            return lines;
        }

        private bool IsCsvFile(string fileName)
        {
            if (!Path.HasExtension(fileName))
                return false;

            return Path.GetExtension(fileName).ToLowerInvariant() == ".csv";
        }

        private string SanitizeFileName(string fileName)
        {
            var invalids = Path.GetInvalidFileNameChars();
            var newFileName = String.Join("_", fileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            newFileName = Path.GetFileNameWithoutExtension(newFileName);

            if (newFileName.Length > 100)
            {
                newFileName = newFileName.Substring(0, 100);
            }

            return newFileName;
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

        private async Task<(Dictionary<string, List<string>> Errors, SubmissionObjectCreateDto Submission)> ValidateAndParseUploadFileAsync(WorkRptUploadDto upload)
        {
            var submission = new SubmissionObjectCreateDto();
            submission.FileName = SanitizeFileName(Path.GetFileName(upload.ReportFile.FileName));
            submission.MimeTypeId = 1;
            submission.ServiceAreaNumber = upload.ServiceAreaNumber;

            var errors = new Dictionary<string, List<string>>();

            var reportType = await _streamService.GetSubmissionStreamByTableNameAsync(TableNames.WorkReport);
            if (reportType == null)
            {
                throw new Exception($"The submission stream for {TableNames.WorkReport} is not defined.");
            }

            if (!IsCsvFile(upload.ReportFile.FileName))
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

                if (row.ServiceArea != serviceArea.ServiceAreaNumber.ToString())
                {
                    errors.AddItem("ServiceArea", $"The file contains service area which is not {serviceArea.ServiceAreaName} ({serviceArea.ServiceAreaNumber}).");
                    return (errors, submission);
                }

                var line = csv.Context.RawRecord.RemoveLineBreak();

                submission.SubmissionRows.Add(new SubmissionRowDto
                {
                    RecordNumber = row.RecordNumber,
                    RowValue = line,
                    RowHash = line.GetSha256Hash(),
                    RowStatusId = RowStatus.Accepted,
                    EndDate = row.EndDate
                });
            }

            HasDuplicateRecordNumberInFile(submission.SubmissionRows, errors);

            await foreach (var row in _rowRepo.FindDuplicateRowsAsync(submission.SubmissionRows))
            {
                row.RowStatusId = RowStatus.Duplicate;
            }

            return (errors, submission); 
        }

        public async Task<(decimal SubmissionObjectId, Dictionary<string, List<string>> Errors)> CreateWorkReportAsync(WorkRptUploadDto upload)
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
