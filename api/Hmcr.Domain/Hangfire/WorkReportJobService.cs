using CsvHelper;
using CsvHelper.TypeConversion;
using Hmcr.Chris;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
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
        Task ProcessSubmission(SubmissionDto submission);
    }

    public class WorkReportJobService : ReportJobServiceBase, IWorkReportJobService
    {
        private IFieldValidatorService _validator;
        private IActivityCodeRepository _activityRepo;
        private IWorkReportRepository _workReportRepo;
        private IMapsApi _mapsApi;
        private IOasApi _oasApi;

        public WorkReportJobService(IUnitOfWork unitOfWork, ILogger<IWorkReportJobService> logger,
            IActivityCodeRepository activityRepo, ISubmissionStatusRepository statusRepo, ISubmissionObjectRepository submissionRepo,
            ISumbissionRowRepository submissionRowRepo, IWorkReportRepository workReportRepo, IFieldValidatorService validator, 
            IEmailService emailService, IConfiguration config, EmailBody emailBody, IFeebackMessageRepository feedbackRepo,
            IMapsApi mapsApi, IOasApi oasApi)
            : base(unitOfWork, statusRepo, submissionRepo, submissionRowRepo, emailService, logger, config, emailBody, feedbackRepo)
        {
            _logger = logger;
            _activityRepo = activityRepo;
            _workReportRepo = workReportRepo;
            _validator = validator;
            _mapsApi = mapsApi;
            _oasApi = oasApi;
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

                untypedRow.PointLineFeature = activityCode.PointLineFeature ?? PointLineFeature.None;

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
                var (rowNum, rows) = ParseRowsTyped(text, errors);

                if (rowNum != 0)
                {
                    var submissionRow = await _submissionRowRepo.GetSubmissionRowByRowNum(_submission.SubmissionObjectId, rowNum);
                    SetErrorDetail(submissionRow, errors);
                    await CommitAndSendEmail();
                    return;
                }

                typedRows = rows;
                await PerformAdditionalValidationAsync(typedRows);
            }

            if (_submission.SubmissionStatusId == _errorFileStatusId)
            {
                await CommitAndSendEmail();
                return;
            }

            //Spatial Validation and Conversion

            _submission.SubmissionStatusId = _successFileStatusId;

            await foreach (var entity in _workReportRepo.SaveWorkReportAsnyc(_submission, typedRows)) { }

            await CommitAndSendEmail();

        }

        private void CopyPointLineFeatureFormUntypedRow(List<WorkReportDto> typedRows, List<WorkReportCsvDto> untypedRows)
        {
            foreach(var typedRow in typedRows)
            {
                var untypedRow = untypedRows.First(x => x.RowNum == typedRow.RowNum);
                typedRow.PointLineFeature = untypedRow.PointLineFeature;
            }
        }

        private async IAsyncEnumerable<WorkReportDto> PerformSpatialValidationAndConversionAsync(List<WorkReportDto> typedRows)
        {
            foreach (var typedRow in typedRows)
            {
                if (typedRow.PointLineFeature == PointLineFeature.None)
                    continue;

                if (typedRow.PointLineFeature == PointLineFeature.Point)
                {
                }
                else if (typedRow.PointLineFeature == PointLineFeature.Line)
                {
                }
                else if (typedRow.PointLineFeature == PointLineFeature.Either)
                {
                }

                await Task.CompletedTask;

                yield return typedRow;
            }
        }

        private void PerformGpsPointValidation(WorkReportDto typedRow, HmrSubmissionRow submissionRow)
        {
            if (typedRow.StartLatitude == null || typedRow.PointLineFeature != PointLineFeature.Point)
                return;

            if (typedRow.EndLatitude != null && typedRow.EndLongitude != null)
            {
                if (typedRow.EndLatitude != typedRow.StartLatitude || typedRow.EndLongitude != typedRow.StartLongitude)
                {
                    var errors = new Dictionary<string, List<string>>();
                    errors.AddItem($"{Fields.EndLatitude},{Fields.EndLongitude}", "Start GPS coordinate must be the same as end GPS coordinate");
                    SetErrorDetail(submissionRow, errors);
                }
            }
            else
            {
                typedRow.EndLatitude = typedRow.StartLatitude;
                typedRow.EndLongitude = typedRow.StartLongitude;
            }
        }

        private void PerformGpsLineValidation(WorkReportDto typedRow, HmrSubmissionRow submissionRow)
        {
            if (typedRow.StartLatitude == null || typedRow.PointLineFeature != PointLineFeature.Line)
                return;

            if (typedRow.EndLatitude != null && typedRow.EndLongitude != null)
            {
                if (typedRow.EndLatitude == typedRow.StartLatitude || typedRow.EndLongitude == typedRow.StartLongitude)
                {
                    var errors = new Dictionary<string, List<string>>();
                    errors.AddItem($"{Fields.EndLatitude},{Fields.EndLongitude}", "The start GPS coordinate must not be the same as the end GPS coordinate");
                    SetErrorDetail(submissionRow, errors);
                }
            }
            else
            {
                var errors = new Dictionary<string, List<string>>();
                errors.AddItem($"{Fields.EndLatitude},{Fields.EndLongitude}", "The end GPS coordinate must not be provided");
                SetErrorDetail(submissionRow, errors);
            }
        }

        private void PerformGpsEitherLineOrPointValidation(WorkReportDto typedRow)
        {
            if (typedRow.StartLatitude == null || typedRow.PointLineFeature != PointLineFeature.Either)
                return;

            if (typedRow.EndLatitude == null || typedRow.EndLongitude == null)
            {
                typedRow.EndLatitude = typedRow.StartLatitude;
                typedRow.EndLongitude = typedRow.StartLongitude;
            }
        }

        private void PerformOffsetPointValidation(WorkReportDto typedRow, HmrSubmissionRow submissionRow)
        {
            if (typedRow.StartOffset == null || typedRow.PointLineFeature != PointLineFeature.Point)
                return;

            if (typedRow.EndOffset != null)
            {
                if (typedRow.EndOffset != typedRow.StartOffset)
                {
                    var errors = new Dictionary<string, List<string>>();
                    errors.AddItem($"{Fields.EndOffset}", "End offset must be the same as start offset");
                    SetErrorDetail(submissionRow, errors);
                }
            }
            else
            {
                typedRow.EndOffset = typedRow.StartOffset;
            }
        }

        private void PerformOffsetLineValidation(WorkReportDto typedRow, HmrSubmissionRow submissionRow)
        {
            if (typedRow.StartOffset == null || typedRow.PointLineFeature != PointLineFeature.Line)
                return;

            if (typedRow.EndOffset != null)
            {
                if (typedRow.StartOffset >= typedRow.EndOffset)
                {
                    var errors = new Dictionary<string, List<string>>();
                    errors.AddItem($"{Fields.EndOffset}", "End offset must be greater than start offset");
                    SetErrorDetail(submissionRow, errors);
                }
            }
            else
            {
                var errors = new Dictionary<string, List<string>>();
                errors.AddItem($"{Fields.EndOffset}", "End offset must be provided");
                SetErrorDetail(submissionRow, errors);
            }
        }

        private void PerformOffsetEitherLineOrPointValidation(WorkReportDto typedRow)
        {
            if (typedRow.StartOffset == null || typedRow.PointLineFeature != PointLineFeature.Either)
                return;

            if (typedRow.EndOffset != null)
            {
                typedRow.EndOffset = typedRow.StartOffset;
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

                PerformGpsPointValidation(typedRow, submissionRow);
                PerformGpsLineValidation(typedRow, submissionRow);
                PerformGpsEitherLineOrPointValidation(typedRow);

                PerformOffsetPointValidation(typedRow, submissionRow);
                PerformOffsetLineValidation(typedRow, submissionRow);
                PerformOffsetEitherLineOrPointValidation(typedRow);

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
            for (var i = 0; i < rows.Count; i++)
            {
                rows[i].RowNum = i + 1;
            }

            return (rows, GetHeader(text));
        }

        private (decimal rowNum, List<WorkReportDto> rows) ParseRowsTyped(string text, Dictionary<string, List<string>> errors)
        {
            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WorkReportDtoMap>();

            var rows = new List<WorkReportDto>();
            var rowNum = 0M;

            while (csv.Read())
            {
                try
                {
                    var row = csv.GetRecord<WorkReportDto>();
                    rows.Add(row);
                    rowNum = (decimal)row.RowNum;
                }
                catch (Exception ex)
                {
                    rowNum = GetRowNum(csv.Context.RawRecord);
                    LogRowParseException(rowNum, ex.ToString(), csv.Context);
                    errors.AddItem("Parse Error", "Exception while parsing");
                    return (rowNum, null);
                }
            }

            return (0, rows);
        }
    }
}
