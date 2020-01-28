using CsvHelper;
using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Domain.Hangfire.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.ActivityCode;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.WorkReport;
using Hmcr.Model.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Hangfire
{
    public interface IWorkReportJobService
    {
        Task ProcessSubmission(SubmissionDto submission);
    }

    public class WorkReportJobService : ReportJobServiceBase, IWorkReportJobService
    {
        private IFieldValidatorService _validator;
        private IActivityCodeRepository _activityRepo;
        private IWorkReportRepository _workReportRepo;

        public WorkReportJobService(IUnitOfWork unitOfWork, ILogger<IWorkReportJobService> logger,
            IActivityCodeRepository activityRepo, ISubmissionStatusRepository statusRepo, ISubmissionObjectRepository submissionRepo,
            ISumbissionRowRepository submissionRowRepo, IWorkReportRepository workReportRepo, IFieldValidatorService validator, 
            IEmailService emailService, IConfiguration config, EmailBody emailBody)
            : base(unitOfWork, statusRepo, submissionRepo, submissionRowRepo, emailService, logger, config, emailBody)
        {
            _logger = logger;
            _activityRepo = activityRepo;
            _workReportRepo = workReportRepo;
            _validator = validator;
        }

        public async Task ProcessSubmission(SubmissionDto submissionDto)
        {
            var errors = new Dictionary<string, List<string>>();

            await SetStatusesAsync();
            await SetSubmissionAsync(submissionDto);

            var activityCodes = await _activityRepo.GetActiveActivityCodesAsync();

            var (untypedRows, headers) = ParseRowsUnTyped(errors);

            if (!CheckCommonMandatoryHeaders(untypedRows, new WorkReportHeaders(), errors))
            {
                if (errors.Count > 0)
                {
                    _submission.ErrorDetail = errors.GetErrorDetail();
                    _submission.SubmissionStatusId = _errorFileStatusId;
                    await CommitAndSendEmail();
                    return;
                }
            }

            //text after duplicate lines are removed. Will be used for importing to typed DTO.
            var text = await SetRowIdAndRemoveDuplicate(untypedRows, headers);

            foreach(var untypedRow in untypedRows)
            {
                errors = new Dictionary<string, List<string>>();

                var submissionRow = await _submissionRowRepo.GetSubmissionRowByRowId(untypedRow.RowId);
                submissionRow.RowStatusId = _successRowStatusId; //set the initial row status as success 

                var activityCode = activityCodes.FirstOrDefault(x => x.ActivityNumber == untypedRow.ActivityNumber);

                if (activityCode == null)
                {
                    errors.AddItem(Fields.ActivityNumber, $"Invalid activity number[{untypedRow.ActivityNumber}]");
                    SetErrorDetail(submissionRow, errors);
                    continue;
                }

                var entityName = GetValidationEntityName(untypedRow, activityCode);

                _validator.Validate(entityName, untypedRow, errors);

                if (errors.Count > 0)
                {
                    SetErrorDetail(submissionRow, errors);
                }
            }

            var typedRows = new List<WorkReportDto>();

            if (_submission.SubmissionStatusId != _errorFileStatusId)
            {
                typedRows = ParseRowsTyped(text, errors);
                await PerformAdditionalValidationAsync(typedRows);
            }

            if (_submission.SubmissionStatusId == _errorFileStatusId)
            {
                await CommitAndSendEmail();
            }
            else
            {
                _submission.SubmissionStatusId = _successFileStatusId;

                await foreach (var entity in _workReportRepo.SaveWorkReportAsnyc(_submission, typedRows)) { }

                await CommitAndSendEmail();
            }
        }

        private async Task PerformAdditionalValidationAsync(List<WorkReportDto> typedRows)
        {
            foreach (var typedRow in typedRows)
            {
                var errors = new Dictionary<string, List<string>>();
                var submissionRow = await _submissionRowRepo.GetSubmissionRowByRowNum(_submission.SubmissionObjectId, (decimal)typedRow.RowNum);

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
                    SetErrorDetail(submissionRow, errors);
                }
            }
        }

        private string GetValidationEntityName(WorkReportCsvDto untypedRow, ActivityCodeDto activityCode)
        {
            var isSite = activityCode.ActivityNumber.Substring(0, 1) == "6";

            var locationCode = activityCode.LocationCode;

            string entityName;
            if (locationCode.LocationCode == "C")
            {
                if (untypedRow.EndLatitude.IsEmpty())
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

        private (List<WorkReportCsvDto> untypedRows, string headers) ParseRowsUnTyped(Dictionary<string, List<string>> errors)
        {
            var text = Encoding.UTF8.GetString(_submission.DigitalRepresentation);

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
