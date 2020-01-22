using CsvHelper;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.WildlifeReport;
using Hmcr.Model.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Hmcr.Domain.Hangfire
{
    public interface IWildlifeReportJobService
    {
        Task ProcessSubmission(SubmissionDto submission);
    }

    public class WildlifeReportJobService : IWildlifeReportJobService
    {
        protected IUnitOfWork _unitOfWork;
        private ILogger _logger;
        protected IFieldValidatorService _validator;
        protected ISubmissionStatusRepository _statusRepo;
        private ISubmissionObjectRepository _submissionRepo;
        private ISumbissionRowRepository _submissionRowRepo;
        private IWildlifeReportRepository _wildlifeReportRepo;

        public WildlifeReportJobService(IUnitOfWork unitOfWork, ILogger<IWildlifeReportJobService> logger,
            ISubmissionStatusRepository statusRepo, ISubmissionObjectRepository submissionRepo,
            ISumbissionRowRepository submissionRowRepo, IWildlifeReportRepository wildlifeReportRepo, IFieldValidatorService validator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _statusRepo = statusRepo;
            _submissionRepo = submissionRepo;
            _submissionRowRepo = submissionRowRepo;
            _wildlifeReportRepo = wildlifeReportRepo;
            _validator = validator;
        }

        public async Task ProcessSubmission(SubmissionDto submissionDto)
        {
            _logger.LogInformation("[Hangfire] Starting submission {submissionObjectId}", submissionDto.SubmissionObjectId);
            var errors = new Dictionary<string, List<string>>();

            var submission = await _submissionRepo.GetSubmissionObjecForBackgroundJobAsync(submissionDto.SubmissionObjectId);

            var statuses = await _statusRepo.GetActiveStatuses();
            var inProgressRowStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.InProgress).StatusId;
            submission.SubmissionStatusId = inProgressRowStatusId;
            _unitOfWork.Commit();

            var duplicateRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.DuplicateRow).StatusId;
            var errorRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.RowError).StatusId;
            var successRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.Success).StatusId;

            var errorFileStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.DataError).StatusId;
            var successFileStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.Success).StatusId;

            var (untypedRows, headers) = ParseRowsUnTyped(submission, errors);

            if (!CheckCommonMandatoryHeaders(untypedRows, errors))
            {
                if (errors.Count > 0)
                {
                    submission.ErrorDetail = errors.GetErrorDetail();
                    submission.SubmissionStatusId = errorFileStatusId;
                    await _unitOfWork.CommitAsync();
                    return;
                }
            }

            //text after duplicate lines are removed. Will be used for importing to typed DTO.
            var text = await SetRowIdAndRemoveDuplicate(submission, duplicateRowStatusId, untypedRows, headers);

            foreach (var untypedRow in untypedRows)
            {
                errors = new Dictionary<string, List<string>>();
                var submissionRow = await _submissionRowRepo.GetSubmissionRowByRowId(untypedRow.RowId);
                submissionRow.RowStatusId = successRowStatusId; //set the initial row status as success 

                var entityName = GetValidationEntityName(untypedRow);

                _validator.Validate(entityName, untypedRow, errors);

                if (errors.Count > 0)
                {
                    submissionRow.RowStatusId = errorRowStatusId;
                    submissionRow.ErrorDetail = errors.GetErrorDetail();
                    submission.ErrorDetail = FileError.ReferToRowErrors;
                    submission.SubmissionStatusId = errorFileStatusId;
                }
            }

            var typedRows = new List<WildlifeReportDto>();

            if (submission.SubmissionStatusId != errorFileStatusId)
            {
                typedRows = ParseRowsTyped(text, errors);
                await PerformAdditionalValidationAsync(submission, typedRows);
            }

            if (submission.SubmissionStatusId == errorFileStatusId)
            {
                await _unitOfWork.CommitAsync();
            }
            else
            {
                submission.SubmissionStatusId = successFileStatusId;

                _wildlifeReportRepo.SaveWildlifeReportAsnyc(submission, typedRows);

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Report Saved");
            }

            _logger.LogInformation("[Hangfire] Finishing submission {submissionObjectId}", submission.SubmissionObjectId);
        }

        private async Task PerformAdditionalValidationAsync(HmrSubmissionObject submission, List<WildlifeReportDto> typedRows)
        {
            var statuses = await _statusRepo.GetActiveStatuses();

            var errorRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.RowError).StatusId;
            var errorFileStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.DataError).StatusId;

            foreach (var typedRow in typedRows)
            {
                var errors = new Dictionary<string, List<string>>();
                var submissionRow = await _submissionRowRepo.GetSubmissionRowByRowNum(submission.SubmissionObjectId, (decimal)typedRow.RowNum);

                //Geo-spatial Validation here

                if (errors.Count > 0)
                {
                    submissionRow.RowStatusId = errorRowStatusId;
                    submissionRow.ErrorDetail = errors.GetErrorDetail();
                    submission.ErrorDetail = FileError.ReferToRowErrors;
                    submission.SubmissionStatusId = errorFileStatusId;
                }
            }
        }

        private string GetValidationEntityName(WildlifeReportCsvDto row)
        {
            return row.Latitude.IsEmpty() ? Entities.WildlifeReportLrs : Entities.WildlifeReportGps;
        }

        private bool CheckCommonMandatoryHeaders(List<WildlifeReportCsvDto> rows, Dictionary<string, List<string>> errors)
        {
            if (rows.Count == 0) //not possible since it's already validated in the ReportServiceBase.
                throw new Exception("File has no rows.");

            var row = rows[0];

            var fields = typeof(WildlifeReportCsvDto).GetProperties();

            foreach (var field in fields)
            {
                if (!WildlifeReportHeaders.CommonMandatoryFields.Any(x => x == field.Name))
                    continue;

                if (field.GetValue(row) == null)
                {
                    errors.AddItem("File", $"Header [{field.Name.WordToWords()}] is missing");
                }
            }

            return errors.Count == 0;
        }

        private async Task<string> SetRowIdAndRemoveDuplicate(HmrSubmissionObject submission, decimal duplicateStatusId, List<WildlifeReportCsvDto> rows, string headers)
        {
            headers = $"{Fields.RowNum}," + headers;
            var text = new StringBuilder();
            text.AppendLine(headers);

            for (int i = rows.Count - 1; i >= 0; i--)
            {
                var row = rows[i];
                var entity = await _submissionRowRepo.GetSubmissionRowByRowNum(submission.SubmissionObjectId, (decimal)row.RowNum);

                if (entity.RowStatusId == duplicateStatusId)
                {
                    rows.RemoveAt(i);
                    continue;
                }

                text.AppendLine($"{row.RowNum},{entity.RowValue}");
                row.RowId = entity.RowId;
            }

            return text.ToString();
        }

        private (List<WildlifeReportCsvDto> untypedRows, string headers) ParseRowsUnTyped(HmrSubmissionObject submission, Dictionary<string, List<string>> errors)
        {
            var text = Encoding.UTF8.GetString(submission.DigitalRepresentation);

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WildlifeReportCsvDtoMap>();

            var rows = csv.GetRecords<WildlifeReportCsvDto>().ToList();
            for (var i = 0; i < rows.Count; i++)
            {
                rows[i].RowNum = i + 1;
            }

            return (rows, GetHeader(text));
        }

        private string GetHeader(string text)
        {
            if (text == null)
                return "";

            using var reader = new StringReader(text);
            var header = reader.ReadLine().Replace("\"", "");

            return header ?? "";
        }

        private List<WildlifeReportDto> ParseRowsTyped(string text, Dictionary<string, List<string>> errors)
        {
            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WildlifeReportDtoMap>();

            var rows = csv.GetRecords<WildlifeReportDto>().ToList();
            return rows;
        }
    }
}
