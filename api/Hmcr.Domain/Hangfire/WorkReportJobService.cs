﻿using CsvHelper;
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
using NetTopologySuite.Geometries;
using System;
using System.Collections.Concurrent;
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
        Task<bool> ProcessSubmissionMain(SubmissionDto submission);
    }

    public class WorkReportJobService : ReportJobServiceBase, IWorkReportJobService
    {
        private IActivityCodeRepository _activityRepo;
        private IWorkReportRepository _workReportRepo;

        public WorkReportJobService(IUnitOfWork unitOfWork, ILogger<IWorkReportJobService> logger,
            IActivityCodeRepository activityRepo, ISubmissionStatusRepository statusRepo, ISubmissionObjectRepository submissionRepo, IServiceAreaRepository serviceAreaRepo,
            ISumbissionRowRepository submissionRowRepo, IWorkReportRepository workReportRepo, IFieldValidatorService validator,
            IEmailService emailService, IConfiguration config, EmailBody emailBody, IFeebackMessageRepository feedbackRepo,
            ISpatialService spatialService, ILookupCodeService lookupService)
            : base(unitOfWork, statusRepo, submissionRepo, serviceAreaRepo, submissionRowRepo, emailService, logger, config, validator, spatialService, emailBody, feedbackRepo, lookupService)
        {
            _activityRepo = activityRepo;
            _workReportRepo = workReportRepo;
        }

        /// <summary>
        /// Returns if it can continue to the next submission or not. 
        /// When it encounters a concurrency issue - when there are more than one job for the same service area, 
        /// one of them must stop and the return value indicates whether to continue or not.
        /// </summary>
        /// <param name="submissionDto"></param>
        /// <returns></returns>
        public override async Task<bool> ProcessSubmission(SubmissionDto submissionDto)
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
                    await CommitAndSendEmailAsync();
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
                await CommitAndSendEmailAsync();
                return true;
            }

            foreach (var untypedRow in untypedRows)
            {
                errors = new Dictionary<string, List<string>>();

                var submissionRow = _submissionRows[(decimal)untypedRow.RowNum];

                submissionRow.RowStatusId = _successRowStatusId; //set the initial row status as success 

                var activityCode = activityCodes.FirstOrDefault(x => x.ActivityNumber == untypedRow.ActivityNumber);

                if (activityCode == null)
                {
                    errors.AddItem(Fields.ActivityNumber, $"Invalid activity number[{untypedRow.ActivityNumber}]");
                    SetErrorDetail(submissionRow, errors);
                    continue;
                }

                untypedRow.FeatureType = activityCode.FeatureType ?? FeatureType.None;
                untypedRow.SpThresholdLevel = activityCode.SpThresholdLevel;

                //this also sets RowType (D2, D3, D4)
                var entityName = GetValidationEntityName(untypedRow, activityCode);

                _validator.Validate(entityName, untypedRow, errors);

                PerformFieldValidation(errors, untypedRow, activityCode);

                if (errors.Count > 0)
                {
                    SetErrorDetail(submissionRow, errors);
                }
            }

            var typedRows = new List<WorkReportTyped>();

            if (_submission.SubmissionStatusId != _errorFileStatusId)
            {
                var (rowNum, rows) = ParseRowsTyped(text, errors);

                if (rowNum != 0)
                {
                    var submissionRow = _submissionRows[rowNum];
                    SetErrorDetail(submissionRow, errors);
                    await CommitAndSendEmailAsync();
                    return true;
                }

                typedRows = rows;

                CopyCalculatedFieldsFormUntypedRow(typedRows, untypedRows);

                PerformAdditionalValidation(typedRows);
            }

            if (_submission.SubmissionStatusId == _errorFileStatusId)
            {
                await CommitAndSendEmailAsync();
                return true;
            }

            var workReports = PerformSpatialValidationAndConversionBatchAsync(typedRows);

            _logger.LogInformation($"{_methodLogHeader} PerformSpatialValidationAndConversionAsync 100%");

            if (_submission.SubmissionStatusId == _errorFileStatusId)
            {
                await CommitAndSendEmailAsync();
                return true;
            }

            _submission.SubmissionStatusId = _successFileStatusId;

            await foreach (var entity in _workReportRepo.SaveWorkReportAsnyc(_submission, workReports)) { }

            await CommitAndSendEmailAsync();

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

            if (untypedRow.UnitOfMeasure.ToLowerInvariant() != activityCode.UnitOfMeasure.ToLowerInvariant())
            {
                errors.AddItem(Fields.UnitOfMeasure, $"Unit of measure for the activity Code [{activityCode.ActivityNumber}] must be [{activityCode.UnitOfMeasure}]");
            }

            if (untypedRow.RecordType.ToLowerInvariant() != activityCode.MaintenanceType.ToLowerInvariant())
            {
                errors.AddItem(Fields.RecordType, $"Record type of the activity code [{activityCode.ActivityNumber}] must be [{activityCode.MaintenanceType}]");
            }
        }

        private void PerformAdditionalValidation(List<WorkReportTyped> typedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var errors = new Dictionary<string, List<string>>();
                var submissionRow = _submissionRows[(decimal)typedRow.RowNum];

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

                ValidateHighwayUniqueAgainstServiceArea(typedRow.HighwayUnique, errors);

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

                if (!ValidateGpsCoordsRange(typedRow.StartLongitude, typedRow.StartLatitude))
                {
                    errors.AddItem($"{Fields.StartLongitude}/{Fields.StartLatitude}", "Invalid range of GPS coordinates.");
                }

                if (!ValidateGpsCoordsRange(typedRow.EndLongitude, typedRow.EndLatitude))
                {
                    errors.AddItem($"{Fields.EndLongitude}/{Fields.EndLatitude}", "Invalid range of GPS coordinates.");
                }

                if (errors.Count > 0)
                {
                    SetErrorDetail(submissionRow, errors);
                }
            }
        }

        private void CopyCalculatedFieldsFormUntypedRow(List<WorkReportTyped> typedRows, List<WorkReportCsvDto> untypedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var untypedRow = untypedRows.First(x => x.RowNum == typedRow.RowNum);
                typedRow.FeatureType = untypedRow.FeatureType;
                typedRow.SpatialData = untypedRow.SpatialData;
                typedRow.RowId = untypedRow.RowId;
                typedRow.SpThresholdLevel = untypedRow.SpThresholdLevel;
            }
        }

        private List<WorkReportGeometry> PerformSpatialValidationAndConversionBatchAsync(List<WorkReportTyped> typedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader, $"Total Record: {typedRows.Count}");

            //grouping the rows
            var groups = new List<List<WorkReportTyped>>();
            var currentGroup = new List<WorkReportTyped>();

            var count = 0;
            foreach (var typedRow in typedRows)
            {
                currentGroup.Add(typedRow);
                count++;

                if (count % 10 == 0)
                {
                    groups.Add(currentGroup);
                    currentGroup = new List<WorkReportTyped>();
                }
            }

            if (currentGroup.Count > 0)
            {
                groups.Add(currentGroup);
            }

            var geometries = new ConcurrentBag<WorkReportGeometry>();
            var progress = 0;

            foreach (var group in groups)
            {
                var tasklist = new List<Task>();

                foreach (var row in group)
                {
                    tasklist.Add(Task.Run(async () => geometries.Add(await PerformSpatialValidationAndConversionAsync(row))));
                }

                Task.WaitAll(tasklist.ToArray());

                progress += 10;

                if (progress % 500 == 0)
                {
                    _logger.LogInformation($"{_methodLogHeader} PerformSpatialValidationAndConversionAsync {progress}");
                }
            }

            return geometries.ToList();
        }

        private async Task<WorkReportGeometry> PerformSpatialValidationAndConversionAsync(WorkReportTyped typedRow)
        {
            var submissionRow = _submissionRows[(decimal)typedRow.RowNum];
            var workReport = new WorkReportGeometry(typedRow, null);

            if (typedRow.SpatialData == SpatialData.Gps)
            {
                await PerformSpatialGpsValidation(workReport, submissionRow);

                SetVarianceWarningDetail(submissionRow, typedRow.HighwayUnique,
                    GetGpsString(typedRow.StartLatitude, typedRow.StartLongitude),
                    GetGpsString(typedRow.EndLatitude, typedRow.EndLongitude),
                    typedRow.SpThresholdLevel);
            }
            else if (typedRow.SpatialData == SpatialData.Lrs)
            {
                await PerformSpatialLrsValidation(workReport, submissionRow);

                SetVarianceWarningDetail(submissionRow, typedRow.HighwayUnique,
                    GetOffsetString(typedRow.StartOffset),
                    GetOffsetString(typedRow.EndOffset),
                    typedRow.SpThresholdLevel);
            }

            return workReport;
        }

        private async Task PerformSpatialGpsValidation(WorkReportGeometry workReport, HmrSubmissionRow submissionRow)
        {
            var errors = new Dictionary<string, List<string>>();
            var typedRow = workReport.WorkReportTyped;

            var start = new Chris.Models.Point((decimal)typedRow.StartLongitude, (decimal)typedRow.StartLatitude);

            //remeber that feature type line/point has been replaced either line or point in PerformGpsEitherLineOrPointValidation().
            if (typedRow.FeatureType == FeatureType.Point)
            {
                var result = await _spatialService.ValidateGpsPointAsync(start, typedRow.HighwayUnique, Fields.HighwayUnique, typedRow.SpThresholdLevel, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors);
                }
                else if (result.result == SpValidationResult.Success)
                {
                    typedRow.HighwayUniqueLength = result.rfiSegment.Length;
                    typedRow.HighwayUniqueName = result.rfiSegment.Descr;

                    typedRow.StartOffset = result.lrsResult.Offset;
                    workReport.Geometry = _geometryFactory.CreatePoint(result.lrsResult.SnappedPoint.ToTopologyCoordinate());
                    submissionRow.StartVariance = result.lrsResult.Variance;
                }
            }
            else if (typedRow.FeatureType == FeatureType.Line)
            {
                var end = new Chris.Models.Point((decimal)typedRow.EndLongitude, (decimal)typedRow.EndLatitude);
                var result = await _spatialService.ValidateGpsLineAsync(start, end, typedRow.HighwayUnique, Fields.HighwayUnique, typedRow.SpThresholdLevel, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors);
                }
                else if (result.result == SpValidationResult.Success)
                {
                    typedRow.HighwayUniqueLength = result.rfiSegment.Length;
                    typedRow.HighwayUniqueName = result.rfiSegment.Descr;

                    typedRow.StartOffset = result.startPointResult.Offset;
                    submissionRow.StartVariance = result.startPointResult.Variance;

                    typedRow.EndOffset = result.endPointResult.Offset;
                    submissionRow.EndVariance = result.endPointResult.Variance;

                    typedRow.WorkLength = typedRow.EndOffset - typedRow.StartOffset;

                    if (result.lines.Count == 1)
                    {
                        if (result.lines[0].ToTopologyCoordinates().Length >= 2)
                        {
                            workReport.Geometry = _geometryFactory.CreateLineString(result.lines[0].ToTopologyCoordinates());
                        }
                        else if (result.lines[0].ToTopologyCoordinates().Length == 1)
                        {
                            _logger.LogInformation($"[Hangfire] Row [{typedRow.RowNum}] [Original: Start[{typedRow.StartLongitude}/{typedRow.StartLatitude}]"
                                + $" End[{typedRow.EndLongitude}/{typedRow.EndLatitude}] were converted to a point [{result.lines[0].Points[0].Longitude}/{result.lines[0].Points[0].Latitude}]");

                            workReport.Geometry = _geometryFactory.CreatePoint(result.lines[0].ToTopologyCoordinates()[0]);
                        }
                    }
                    else if (result.lines.Count > 1)
                    {
                        var lineStrings = new List<LineString>();
                        foreach (var line in result.lines)
                        {
                            lineStrings.Add(_geometryFactory.CreateLineString(line.ToTopologyCoordinates()));
                        }

                        workReport.Geometry = _geometryFactory.CreateMultiLineString(lineStrings.ToArray());
                    }
                }
            }
        }

        private async Task PerformSpatialLrsValidation(WorkReportGeometry workReport, HmrSubmissionRow submissionRow)
        {
            var errors = new Dictionary<string, List<string>>();
            var typedRow = workReport.WorkReportTyped;

            //remeber that feature type line/point has been replaced either line or point in PerformGpsEitherLineOrPointValidation().
            if (typedRow.FeatureType == FeatureType.Point)
            {
                var result = await _spatialService.ValidateLrsPointAsync((decimal)typedRow.StartOffset, typedRow.HighwayUnique, Fields.HighwayUnique, typedRow.SpThresholdLevel, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors);
                }
                else if (result.result == SpValidationResult.Success)
                {
                    typedRow.HighwayUniqueLength = result.rfiSegment.Length;
                    typedRow.HighwayUniqueName = result.rfiSegment.Descr;

                    typedRow.StartLongitude = result.point.Longitude;
                    typedRow.StartLatitude = result.point.Latitude;
                    workReport.Geometry = _geometryFactory.CreatePoint(result.point.ToTopologyCoordinate());
                    submissionRow.StartVariance = typedRow.StartOffset - result.snappedOffset;
                }
            }
            else if (typedRow.FeatureType == FeatureType.Line)
            {
                var result = await _spatialService
                    .ValidateLrsLineAsync((decimal)typedRow.StartOffset, (decimal)typedRow.EndOffset, typedRow.HighwayUnique, Fields.HighwayUnique, typedRow.SpThresholdLevel, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors);
                }
                else if (result.result == SpValidationResult.Success)
                {
                    typedRow.HighwayUniqueLength = result.rfiSegment.Length;
                    typedRow.HighwayUniqueName = result.rfiSegment.Descr;

                    typedRow.StartLongitude = result.startPoint.Longitude;
                    typedRow.StartLatitude = result.startPoint.Latitude;
                    submissionRow.StartVariance = typedRow.StartOffset - result.snappedStartOffset;

                    typedRow.EndLongitude = result.endPoint.Longitude;
                    typedRow.EndLatitude = result.endPoint.Latitude;
                    submissionRow.EndVariance = typedRow.EndOffset - result.snappedEndOffset;

                    typedRow.WorkLength = result.snappedEndOffset - result.snappedStartOffset;

                    if (result.lines.Count == 1)
                    {
                        if (result.lines[0].ToTopologyCoordinates().Length >= 2)
                        {
                            workReport.Geometry = _geometryFactory.CreateLineString(result.lines[0].ToTopologyCoordinates());
                        }
                        else if (result.lines[0].ToTopologyCoordinates().Length == 1)
                        {
                            _logger.LogInformation($"[Hangfire] Row [{typedRow.RowNum}] [Original: Start[{typedRow.StartOffset}]"
                                + $" End[{typedRow.EndOffset}] were converted to a Start[{result.snappedStartOffset}] End[{result.snappedEndOffset}]");

                            workReport.Geometry = _geometryFactory.CreatePoint(result.lines[0].ToTopologyCoordinates()[0]);
                        }
                    }
                    else if (result.lines.Count > 1)
                    {
                        var lineStrings = new List<LineString>();
                        foreach (var line in result.lines)
                        {
                            lineStrings.Add(_geometryFactory.CreateLineString(line.ToTopologyCoordinates()));
                        }

                        workReport.Geometry = _geometryFactory.CreateMultiLineString(lineStrings.ToArray());
                    }
                }
            }
        }

        private void PerformGpsPointValidation(WorkReportTyped typedRow, HmrSubmissionRow submissionRow)
        {
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
                errors.AddItem($"{Fields.EndLatitude}/{Fields.EndLongitude}", "Start GPS coordinates must be the same as end GPS coordinate");
                SetErrorDetail(submissionRow, errors);
            }
        }

        private void PerformGpsLineValidation(WorkReportTyped typedRow, HmrSubmissionRow submissionRow)
        {
            if (typedRow.StartLatitude == null || typedRow.StartLongitude == null || typedRow.FeatureType != FeatureType.Line)
                return;

            if (typedRow.EndLatitude != null && typedRow.EndLongitude != null)
            {
                if (typedRow.EndLatitude == typedRow.StartLatitude && typedRow.EndLongitude == typedRow.StartLongitude)
                {
                    var errors = new Dictionary<string, List<string>>();
                    errors.AddItem($"{Fields.EndLatitude}/{Fields.EndLongitude}", "The start GPS coordinates must not be the same as the end GPS coordinates");
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

        private void PerformGpsEitherLineOrPointValidation(WorkReportTyped typedRow)
        {
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

        private void PerformOffsetPointValidation(WorkReportTyped typedRow, HmrSubmissionRow submissionRow)
        {
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

        private void PerformOffsetLineValidation(WorkReportTyped typedRow, HmrSubmissionRow submissionRow)
        {
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

        private void PerformOffsetEitherLineOrPointValidation(WorkReportTyped typedRow)
        {
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

        private string GetValidationEntityName(WorkReportCsvDto untypedRow, ActivityCodeDto activityCode)
        {
            var locationCode = activityCode.LocationCode;

            string entityName;
            if (locationCode.LocationCode == "C")
            {
                if ((untypedRow.StartLatitude.IsEmpty() || untypedRow.StartLongitude.IsEmpty()) &&
                    !(untypedRow.StartOffset.IsEmpty() || untypedRow.EndOffset.IsEmpty()))
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
                rows[i].RowNum = i + 2;
            }

            return (rows, GetHeader(text));
        }

        private (decimal rowNum, List<WorkReportTyped> rows) ParseRowsTyped(string text, Dictionary<string, List<string>> errors)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WorkReportDtoMap>();

            var rows = new List<WorkReportTyped>();
            var rowNum = 0M;
            while (csv.Read())
            {
                try
                {
                    var row = csv.GetRecord<WorkReportTyped>();
                    rows.Add(row);
                    rowNum = (decimal)row.RowNum;
                }
                catch (CsvHelper.TypeConversion.TypeConverterException ex)
                {
                    _logger.LogError(ex.ToString());
                    rowNum = GetRowNum(csv.Context.RawRecord);
                    LogRowParseException(rowNum, ex.ToString(), csv.Context);
                    errors.AddItem("Parse Error", $"Exception while parsing the text [{ex.Text}]");
                    return (rowNum, null);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    rowNum = GetRowNum(csv.Context.RawRecord);
                    LogRowParseException(rowNum, ex.ToString(), csv.Context);
                    errors.AddItem("Parse Error", $"Exception while parsing");
                    return (rowNum, null);
                }
            }

            return (0, rows);
        }
    }
}
