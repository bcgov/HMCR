using CsvHelper;
using Hmcr.Chris;
using Hmcr.Chris.Models;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Domain.Hangfire.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.LocationCode;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.WorkReport;
using Hmcr.Model.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Hangfire
{
    public interface IWorkReportJobService
    {
        Task<bool> ProcessSubmission(SubmissionDto submission);
    }

    public class WorkReportJobService : ReportJobServiceBase, IWorkReportJobService
    {
        private IFieldValidatorService _validator;
        private ISpatialService _spatialService;
        private GeometryFactory _geometryFactory;
        private IActivityCodeRepository _activityRepo;
        private IWorkReportRepository _workReportRepo;

        public WorkReportJobService(IUnitOfWork unitOfWork, ILogger<IWorkReportJobService> logger,
            IActivityCodeRepository activityRepo, ISubmissionStatusRepository statusRepo, ISubmissionObjectRepository submissionRepo,
            ISumbissionRowRepository submissionRowRepo, IWorkReportRepository workReportRepo, IFieldValidatorService validator,
            IEmailService emailService, IConfiguration config, EmailBody emailBody, IFeebackMessageRepository feedbackRepo,
            ISpatialService spatialService)
            : base(unitOfWork, statusRepo, submissionRepo, submissionRowRepo, emailService, logger, config, emailBody, feedbackRepo)
        {
            _logger = logger;
            _activityRepo = activityRepo;
            _workReportRepo = workReportRepo;
            _validator = validator;
            _spatialService = spatialService;
            _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        }

        /// <summary>
        /// Returns if it can continue to the next submission or not. 
        /// When it encounters a concurrency issue - when there are more than one job for the same service area, 
        /// one of them must stop and the return value indicates whether to continue or not.
        /// </summary>
        /// <param name="submissionDto"></param>
        /// <returns></returns>
        public async Task<bool> ProcessSubmission(SubmissionDto submissionDto)
        {
            var errors = new Dictionary<string, List<string>>();

            await SetMemberVariablesAsync();

            if (!await SetSubmissionAsync(submissionDto))
                return false;

            var activityCodes = await _activityRepo.GetActiveActivityCodesAsync();

            var (untypedRows, headers) = ParseRowsUnTyped(errors);

            if (!CheckCommonMandatoryHeaders(untypedRows, new WorkReportHeaders(), errors))
            {
                if (errors.Count > 0)
                {
                    _submission.ErrorDetail = errors.GetErrorDetail();
                    _submission.SubmissionStatusId = _errorFileStatusId;
                    await CommitAndSendEmail();
                    return true;
                }
            }

            //text after duplicate lines are removed. Will be used for importing to typed DTO.
            var (rowCount, text) = await SetRowIdAndRemoveDuplicate(untypedRows, headers);

            if (rowCount == 0)
            {
                errors.AddItem("File", "No new records were found in the file; all records were already processed in the past submission.");
                _submission.ErrorDetail = errors.GetErrorDetail();
                _submission.SubmissionStatusId = _duplicateFileStatusId;
                await CommitAndSendEmail();
                return true;
            }

            foreach (var untypedRow in untypedRows)
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

                untypedRow.FeatureType = activityCode.FeatureType ?? FeatureType.None;

                //this also sets RowType (D2, D3, D4)
                var entityName = GetValidationEntityName(untypedRow, activityCode);

                _validator.Validate(entityName, untypedRow, errors);

                PerformFieldValidation(errors, untypedRow, activityCode);

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
                    return true;
                }

                typedRows = rows;

                CopyCalculatedFieldsFormUntypedRow(typedRows, untypedRows);

                await PerformAdditionalValidationAsync(typedRows);
            }

            if (_submission.SubmissionStatusId == _errorFileStatusId)
            {
                await CommitAndSendEmail();
                return true;
            }

            //Spatial Validation and Conversion
            await foreach (var typedRow in PerformSpatialValidationAndConversionAsync(typedRows))
            {

            }

            _submission.SubmissionStatusId = _successFileStatusId;

            await foreach (var entity in _workReportRepo.SaveWorkReportAsnyc(_submission, typedRows)) { }

            await CommitAndSendEmail();

            return true;
        }

        private void PerformFieldValidation(Dictionary<string, List<string>> errors, WorkReportCsvDto untypedRow, ActivityCodeDto activityCode)
        {
            if (activityCode.LocationCode.LocationCode == "C" && activityCode.ActivityNumber.StartsWith('6'))
            {
                _validator.Validate(Entities.WorkReportStructure, Fields.StructureNumber, untypedRow.StructureNumber, errors);
            }

            if (activityCode.IsSiteNumRequired)
            {
                _validator.Validate(Entities.WorkReportSite, Fields.SiteNumber, untypedRow.SiteNumber, errors);
            }

            if (untypedRow.SpatialData == SpatialData.None && activityCode.LocationCode.LocationCode == "B")
            {
                _validator.Validate(Entities.WorkReportHighwayUnique, Fields.HighwayUnique, untypedRow.HighwayUnique, errors);
            }

            if (untypedRow.RecordType == "Q")
            {
                _validator.Validate(Entities.WorkReportValueOfWork, Fields.ValueOfWork, untypedRow.ValueOfWork, errors);
            }
        }

        private void CopyCalculatedFieldsFormUntypedRow(List<WorkReportDto> typedRows, List<WorkReportCsvDto> untypedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var untypedRow = untypedRows.First(x => x.RowNum == typedRow.RowNum);
                typedRow.FeatureType = untypedRow.FeatureType;
                typedRow.SpatialData = untypedRow.SpatialData;
                typedRow.RowId = untypedRow.RowId;
            }
        }

        private async IAsyncEnumerable<WorkReportDto> PerformSpatialValidationAndConversionAsync(List<WorkReportDto> typedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var submissionRow = await _submissionRowRepo.GetSubmissionRowByRowNum(_submission.SubmissionObjectId, (decimal)typedRow.RowNum);

                if (typedRow.SpatialData == SpatialData.Gps)
                {
                    if (typedRow.FeatureType == FeatureType.Point)
                    {
                        await PerformSpatialGpsValidation(typedRow, submissionRow);
                    }
                    else
                    {

                    }
                }
                else if (typedRow.SpatialData == SpatialData.Lrs)
                {
                    if (typedRow.FeatureType == FeatureType.Point)
                    {
                        
                    }
                    else
                    {

                    }
                }

                yield return typedRow;
            }
        }

        private async Task PerformSpatialGpsValidation(WorkReportDto typedRow, HmrSubmissionRow submissionRow)
        {
            var errors = new Dictionary<string, List<string>>();

            var start = new Chris.Models.Point((decimal)typedRow.StartLongitude, (decimal)typedRow.StartLatitude);

            //remeber that feature type line/point has been replaced either line or point in PerformGpsEitherLineOrPointValidation().
            if (typedRow.FeatureType == FeatureType.Point)
            {
                var result = await _spatialService.ValidateGpsPointAsync(start, typedRow.HighwayUnique, Fields.HighwayUnique, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors);
                }
                else if (result.result == SpValidationResult.Success)
                {
                    typedRow.StartOffset = result.lrsResult.Offset;
                    //typedRow.Geometry = _geometryFactory.CreatePoint(result.lrsResult.SnappedPoint.ToTopologyCoordinate());                  
                    //todo: typedRow.StartVariance = result.lrsResult.Variance;
                }
            }  
            else if (typedRow.FeatureType == FeatureType.Line)
            {
                var end = new Chris.Models.Point((decimal)typedRow.EndLongitude, (decimal)typedRow.EndLatitude);
                var result = await _spatialService.ValidateGpsLineAsync(start, end, typedRow.HighwayUnique, Fields.HighwayUnique, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors);
                }
                else if (result.result == SpValidationResult.Success)
                {
                    typedRow.StartOffset = result.startPointResult.Offset;
                    //todo: typedRow.StartVariance = result.lrsResult.Variance;

                    typedRow.EndOffset = result.endPointResult.Offset;
                    //todo: typedRow.EndVariance = result.lrsResult.Variance;

                    //typedRow.Geometry = _geometryFactory.CreateLineString(result.line.ToTopologyCoordinates());
                }
            } 
        }

        private void PerformGpsPointValidation(WorkReportDto typedRow, HmrSubmissionRow submissionRow)
        {
            //MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);
            
            //if start is null, it's already set to invalid, no more validation
            if (typedRow.StartLatitude == null || typedRow.StartLongitude == null || typedRow.FeatureType != FeatureType.Point)
                return;

            if (typedRow.EndLatitude == null)
            {
                typedRow.EndLatitude = typedRow.StartLatitude;
            }

            if (typedRow.EndLongitude == null)
            {
                typedRow.EndLongitude = typedRow.StartLongitude;
            }

            if (typedRow.EndLatitude != typedRow.StartLatitude || typedRow.EndLongitude != typedRow.StartLongitude)
            {
                var errors = new Dictionary<string, List<string>>();
                errors.AddItem($"{Fields.EndLatitude},{Fields.EndLongitude}", "Start GPS coordinates must be the same as end GPS coordinate");
                SetErrorDetail(submissionRow, errors);
            }
        }

        private void PerformGpsLineValidation(WorkReportDto typedRow, HmrSubmissionRow submissionRow)
        {
            //MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            if (typedRow.StartLatitude == null || typedRow.StartLongitude == null || typedRow.FeatureType != FeatureType.Line)
                return;

            if (typedRow.EndLatitude != null && typedRow.EndLongitude != null)
            {
                if (typedRow.EndLatitude == typedRow.StartLatitude && typedRow.EndLongitude == typedRow.StartLongitude)
                {
                    var errors = new Dictionary<string, List<string>>();
                    errors.AddItem($"{Fields.EndLatitude},{Fields.EndLongitude}", "The start GPS coordinates must not be the same as the end GPS coordinates");
                    SetErrorDetail(submissionRow, errors);
                }
            }
            else
            {
                var errors = new Dictionary<string, List<string>>();
                errors.AddItem($"{Fields.EndLatitude},{Fields.EndLongitude}", "The end GPS coordinates must be provided");
                SetErrorDetail(submissionRow, errors);
            }
        }

        private void PerformGpsEitherLineOrPointValidation(WorkReportDto typedRow)
        {
            //MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            if (typedRow.StartLatitude == null || typedRow.StartLongitude == null || typedRow.FeatureType != FeatureType.PointLine)
                return;

            if (typedRow.EndLatitude == null)
            {
                typedRow.EndLatitude = typedRow.StartLatitude;
            }

            if (typedRow.EndLongitude == null)
            {
                typedRow.EndLongitude = typedRow.StartLongitude;
            }

            if (typedRow.StartLatitude == typedRow.EndLatitude && typedRow.StartLongitude == typedRow.EndLongitude)
            {
                typedRow.FeatureType = FeatureType.Point;
            }
            else
            {
                typedRow.FeatureType = FeatureType.Line;
            }
        }

        private void PerformOffsetPointValidation(WorkReportDto typedRow, HmrSubmissionRow submissionRow)
        {
            //MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            if (typedRow.StartOffset == null || typedRow.FeatureType != FeatureType.Point)
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
            //MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            if (typedRow.StartOffset == null || typedRow.FeatureType != FeatureType.Line)
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
            //MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            if (typedRow.StartOffset == null || typedRow.FeatureType != FeatureType.PointLine)
                return;

            if (typedRow.EndOffset == null)
            {
                typedRow.EndOffset = typedRow.StartOffset;
            }

            if (typedRow.StartOffset == typedRow.EndOffset)
            {
                typedRow.FeatureType = FeatureType.Point;
            }
            else
            {
                typedRow.FeatureType = FeatureType.Line;
            }
        }

        private async Task PerformAdditionalValidationAsync(List<WorkReportDto> typedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var errors = new Dictionary<string, List<string>>();
                var submissionRow = await _submissionRowRepo.GetSubmissionRowByRowNum(_submission.SubmissionObjectId, (decimal)typedRow.RowNum);

                if (typedRow.StartDate != null && typedRow.EndDate < typedRow.StartDate)
                {
                    errors.AddItem("StartDate", "Start Date cannot be greater than End Date");
                }

                if (typedRow.StartDate != null && typedRow.StartDate > DateTime.Now)
                {
                    errors.AddItem(Fields.StartDate, "Cannot be a future date.");
                }

                if (typedRow.EndDate != null && typedRow.EndDate > DateTime.Now)
                {
                    errors.AddItem(Fields.EndDate, "Cannot be a future date.");
                }

                if (typedRow.SpatialData == SpatialData.Gps)
                {
                    PerformGpsPointValidation(typedRow, submissionRow);
                    PerformGpsLineValidation(typedRow, submissionRow);
                    PerformGpsEitherLineOrPointValidation(typedRow);
                }

                if (typedRow.SpatialData == SpatialData.Lrs)
                {
                    PerformOffsetPointValidation(typedRow, submissionRow);
                    PerformOffsetLineValidation(typedRow, submissionRow);
                    PerformOffsetEitherLineOrPointValidation(typedRow);
                }

                if (errors.Count > 0)
                {
                    SetErrorDetail(submissionRow, errors);
                }
            }
        }

        private string GetValidationEntityName(WorkReportCsvDto untypedRow, ActivityCodeDto activityCode)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader, $"RowNum: {untypedRow.RowNum}");

            var locationCode = activityCode.LocationCode;

            string entityName;
            if (locationCode.LocationCode == "C")
            {
                if (untypedRow.StartLatitude.IsEmpty() || untypedRow.StartLongitude.IsEmpty())
                {
                    entityName = Entities.WorkReportD4;
                    untypedRow.SpatialData = SpatialData.Lrs;
                }
                else
                {
                    entityName = Entities.WorkReportD3;
                    untypedRow.SpatialData = SpatialData.Gps;
                }
            }
            else
            {
                entityName = Entities.WorkReportD2;
                untypedRow.SpatialData = SpatialData.None;
            }

            return entityName;
        }

        private (List<WorkReportCsvDto> untypedRows, string headers) ParseRowsUnTyped(Dictionary<string, List<string>> errors)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            var text = Encoding.UTF8.GetString(_submission.DigitalRepresentation);

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);

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
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);

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
                    _logger.LogError(ex.ToString());
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
