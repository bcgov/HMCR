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
        Task ProcessSubmission(HmrSubmissionObject submission, IEnumerable<ActivityCodeDto> activityCodes, IEnumerable<SubmissionStatusDto> statuses);
    }

    public class WorkReportJobService : IWorkReportJobService
    {
        protected IUnitOfWork _unitOfWork;
        private ILogger _logger;
        protected IFieldValidatorService _validator;
        protected ISubmissionStreamService _streamService;
        protected ISubmissionObjectRepository _submissionRepo;
        protected ISumbissionRowRepository _rowRepo;
        protected IContractTermRepository _contractRepo;
        protected ISubmissionStatusRepository _statusRepo;

        public WorkReportJobService(IUnitOfWork unitOfWork, ILogger<IWorkReportJobService> logger,
            ISubmissionStreamService streamService, ISubmissionObjectRepository submissionRepo, ISumbissionRowRepository rowRepo,
            IContractTermRepository contractRepo, ISubmissionStatusRepository statusRepo, IFieldValidatorService validator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _streamService = streamService;
            _submissionRepo = submissionRepo;
            _rowRepo = rowRepo;
            _contractRepo = contractRepo;
            _statusRepo = statusRepo;
            _validator = validator;
        }

        public async Task ProcessSubmission(HmrSubmissionObject submission, IEnumerable<ActivityCodeDto> activityCodes, IEnumerable<SubmissionStatusDto> statuses)
        {
            _logger.LogInformation("[Hangfire] Starting submission {submissionObjectId}", submission.SubmissionObjectId);
            var errors = new Dictionary<string, List<string>>();

            var duplicateRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.Duplicate).StatusId;
            var errorRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.Error).StatusId;
            var errorFileStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.Error).StatusId;

            Console.WriteLine($"Hangfire job {submission.SubmissionObjectId}");

            var rows = ParseRowsUnTyped(submission, errors);

            if (!CheckCommonMandatoryHeaders(rows, errors))
            {
                if (errors.Count > 0)
                {
                    submission.ErrorDetail = errors.GetErrorDetail();
                    submission.SubmissionStatusId = errorFileStatusId;
                    await _unitOfWork.CommitAsync();
                    return;
                }
            }

            SetRowIdAndRemoveDuplicate(submission, duplicateRowStatusId, rows);

            //validate and populate staged row
            foreach(var row in rows)
            {
                //reset errors
                errors = new Dictionary<string, List<string>>();
                var submissionRow = submission.HmrSubmissionRows.First(x => x.RowId == row.RowId);

                var activityCode = activityCodes.FirstOrDefault(x => x.ActivityNumber == row.ActivityNumber);

                if (activityCode == null)
                {
                    submissionRow.RowStatusId = errorRowStatusId;
                    submissionRow.ErrorDetail = $"Row[{row.RowNumber}]: Activity Number[{row.ActivityNumber}] not found"; 
                    submission.ErrorDetail = "Please refer to row errors";
                    submission.SubmissionStatusId = errorFileStatusId;
                    _logger.LogInformation("Row[{rownumber}]: Activity Number[{ativityNumber}] not found", row.RowNumber, row.ActivityNumber);
                    continue;
                }

                var entityName = GetValidationEntityName(row, activityCode);

                _validator.Validate(entityName, row, errors);

                if (errors.Count > 0)
                {
                    submissionRow.RowStatusId = errorRowStatusId;
                    submissionRow.ErrorDetail = $"Row[{row.RowNumber}]: {errors.GetErrorDetail()}";                     
                    submission.ErrorDetail = "Please refer to row error";
                    submission.SubmissionStatusId = errorFileStatusId;
                    _logger.LogInformation("Row[{rownumber}]: {errors}", row.RowNumber, errors.GetErrorDetail());
                }
            }

            if (submission.SubmissionStatusId == errorFileStatusId)
            {
                await _unitOfWork.CommitAsync();
            }
            else
            {
                //submission.SubmissionStatusId = success
                //submission row status = success
                //save
                _logger.LogInformation("Report Saved");
            }

            _logger.LogInformation("[Hangfire] Finishing submission {submissionObjectId}", submission.SubmissionObjectId);
        }

        private string GetValidationEntityName(WorkReportCsvDto row, ActivityCodeDto activityCode)
        {
            var entityName = "";
            var isSite = activityCode.ActivityNumber.Substring(0, 1) == "6";

            var locationCode = activityCode.LocationCode;

            if (locationCode.LocationCode == "C")
            {
                if (row.StartOffset.IsNotEmpty())
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
                entityName = Entities.WorkReportD2;
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

        private void SetRowIdAndRemoveDuplicate(HmrSubmissionObject submission, decimal duplicateStatusId, List<WorkReportCsvDto> rows)
        {
            for (int i = rows.Count - 1; i >= 0; i--)
            {
                var row = rows[i];
                row.RowNumber = i + 1;

                var entity = submission.HmrSubmissionRows.First(x => x.RecordNumber == row.RecordNumber);

                if (entity.RowStatusId == duplicateStatusId)
                {
                    rows.RemoveAt(i);
                    continue;
                }

                row.RowId = entity.RowId;
            }
        }

        private List<WorkReportCsvDto> ParseRowsUnTyped(HmrSubmissionObject submission, Dictionary<string, List<string>> errors)
        {
            var text = Encoding.UTF8.GetString(submission.DigitalRepresentation);

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WorkReportCsvDtoMap>();

            return csv.GetRecords<WorkReportCsvDto>().ToList();
        }
    }
}
