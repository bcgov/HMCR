using CsvHelper;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.ActivityCode;
using Hmcr.Model.Dtos.SubmissionStatus;
using Hmcr.Model.Dtos.WorkReport;
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
    public interface IWorkReportJobService
    {
        Task ProcessSubmission(HmrSubmissionObject submission);
    }

    public class WorkReportJobService : IWorkReportJobService
    {
        private IUnitOfWork _unitOfWork;
        private ILogger _logger;
        private IFieldValidatorService _validator;
        private IActivityCodeRepository _activityRepo;
        private ISubmissionStatusRepository _statusRepo;
        private IWorkReportRepository _workReportRepo;

        public WorkReportJobService(IUnitOfWork unitOfWork, ILogger<IWorkReportJobService> logger,
            IActivityCodeRepository activityRepo, ISubmissionStatusRepository statusRepo, 
            IWorkReportRepository workReportRepo, IFieldValidatorService validator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _activityRepo = activityRepo;
            _statusRepo = statusRepo;
            _workReportRepo = workReportRepo;
            _validator = validator;
        }

        public async Task ProcessSubmission(HmrSubmissionObject submission)
        {
            _logger.LogInformation("[Hangfire] Starting submission {submissionObjectId}", submission.SubmissionObjectId);
            var errors = new Dictionary<string, List<string>>();

            var statuses = await _statusRepo.GetActiveStatuses();
            var inProgressRowStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.InProgress).StatusId;
            submission.SubmissionStatusId = inProgressRowStatusId;
            _unitOfWork.Commit();

            var duplicateRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.DuplicateRow).StatusId;
            var errorRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.RowError).StatusId;
            var successRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.Success).StatusId;

            var errorFileStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.DataError).StatusId;
            var successFileStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.Success).StatusId;

            var activityCodes = await _activityRepo.GetActiveActivityCodesAsync();

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
            var text = SetRowIdAndRemoveDuplicate(submission, duplicateRowStatusId, untypedRows, headers);

            foreach(var untypedRow in untypedRows)
            {
                errors = new Dictionary<string, List<string>>();
                var submissionRow = submission.HmrSubmissionRows.First(x => x.RowId == untypedRow.RowId);
                submissionRow.RowStatusId = successRowStatusId; //set the initial row status as success 

                var activityCode = activityCodes.FirstOrDefault(x => x.ActivityNumber == untypedRow.ActivityNumber);

                if (activityCode == null)
                {
                    errors.AddItem(Fields.ActivityNumber, $"Invalid activity number[{untypedRow.ActivityNumber}]");

                    submissionRow.RowStatusId = errorRowStatusId;
                    submissionRow.ErrorDetail = errors.GetErrorDetail();
                    submission.ErrorDetail = FileError.ReferToRowErrors;
                    submission.SubmissionStatusId = errorFileStatusId;
                    continue;
                }

                var entityName = GetValidationEntityName(untypedRow, activityCode);

                _validator.Validate(entityName, untypedRow, errors);

                if (errors.Count > 0)
                {
                    submissionRow.RowStatusId = errorRowStatusId;
                    submissionRow.ErrorDetail = errors.GetErrorDetail();
                    submission.ErrorDetail = FileError.ReferToRowErrors;
                    submission.SubmissionStatusId = errorFileStatusId;
                }
            }

            var typedRows = new List<WorkReportDto>();

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

                await foreach (var entity in _workReportRepo.SaveWorkReportAsnyc(submission, typedRows)) { }

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Report Saved");
            }

            _logger.LogInformation("[Hangfire] Finishing submission {submissionObjectId}", submission.SubmissionObjectId);
        }

        private async Task PerformAdditionalValidationAsync(HmrSubmissionObject submission, List<WorkReportDto> typedRows)
        {
            var activityCodes = await _activityRepo.GetActiveActivityCodesAsync();
            var statuses = await _statusRepo.GetActiveStatuses();

            var errorRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.RowError).StatusId;
            var errorFileStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.DataError).StatusId;

            foreach (var typedRow in typedRows)
            {
                var errors = new Dictionary<string, List<string>>();
                var submissionRow = submission.HmrSubmissionRows.First(x => x.RowNum == typedRow.RowNum); 

                if (typedRow.StartDate != null && typedRow.EndDate < typedRow.StartDate)
                {
                    errors.AddItem("StartDate", "Start Date cannot be greater than End Date");
                }

                if (typedRow.StartOffset != null && typedRow.EndOffset < typedRow.StartOffset)
                {
                    errors.AddItem("StartOffset", "Start Offset cannot be greater than End Offset");
                }

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

        private string GetValidationEntityName(WorkReportCsvDto row, ActivityCodeDto activityCode)
        {
            var isSite = activityCode.ActivityNumber.Substring(0, 1) == "6";

            var locationCode = activityCode.LocationCode;

            string entityName;
            if (locationCode.LocationCode == "C")
            {
                if (row.EndLatitude.IsEmpty())
                {
                    entityName = isSite ? Entities.WorkReportD4Site : Entities.WorkReportD4;
                }
                else
                {
                    entityName = isSite ? Entities.WorkReportD3Site : Entities.WorkReportD3;
                }
            }
            else
            {
                entityName = (locationCode.LocationCode == "B") ? Entities.WorkReportD2B : Entities.WorkReportD2;
            }

            return entityName;
        }

        private bool CheckCommonMandatoryHeaders(List<WorkReportCsvDto> rows, Dictionary<string, List<string>> errors)
        {
            if (rows.Count == 0) //not possible since it's already validated in the ReportServiceBase.
                throw new Exception("File has no rows.");

            var row = rows[0];

            var fields = typeof(WorkReportCsvDto).GetProperties();

            foreach (var field in fields)
            {
                if (!WorkReportHeaders.CommonMandatoryFields.Any(x => x == field.Name))
                    continue;

                if (field.GetValue(row) == null)
                {
                    errors.AddItem("File", $"Header [{field.Name.WordToWords()}] is missing");
                }
            }

            return errors.Count == 0;
        }

        private string SetRowIdAndRemoveDuplicate(HmrSubmissionObject submission, decimal duplicateStatusId, List<WorkReportCsvDto> rows, string headers)
        {
            headers = $"{Fields.RowNum}," + headers;
            var text = new StringBuilder();
            text.AppendLine(headers);

            for (int i = rows.Count - 1; i >= 0; i--)
            {
                var row = rows[i];
                var entity = submission.HmrSubmissionRows.First(x => x.RowNum == row.RowNum);

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

        private (List<WorkReportCsvDto> untypedRows, string headers) ParseRowsUnTyped(HmrSubmissionObject submission, Dictionary<string, List<string>> errors)
        {
            var text = Encoding.UTF8.GetString(submission.DigitalRepresentation);

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WorkReportCsvDtoMap>();

            var rows = csv.GetRecords<WorkReportCsvDto>().ToList();
            for(var i = 0; i < rows.Count; i++)
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

        private List<WorkReportDto> ParseRowsTyped(string text, Dictionary<string, List<string>> errors)
        {
            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WorkReportDtoMap>();

            var rows = csv.GetRecords<WorkReportDto>().ToList();
            return rows;
        }
    }
}
