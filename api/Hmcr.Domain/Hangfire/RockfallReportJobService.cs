using CsvHelper;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Domain.Hangfire.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.RockfallReport;
using Hmcr.Model.Dtos.SubmissionObject;
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
    public interface IRockfallReportJobService
    {
        Task<bool> ProcessSubmissionMain(SubmissionDto submission);
    }

    public class RockfallReportJobService : ReportJobServiceBase, IRockfallReportJobService
    {        
        private IRockfallReportRepository _rockfallReportRepo;
        private readonly string _thresholdSpLevel = ThresholdSpLevels.Level1;

        public RockfallReportJobService(IUnitOfWork unitOfWork, ILogger<IRockfallReportJobService> logger, 
            ISubmissionStatusService statusService, ISubmissionObjectRepository submissionRepo, IServiceAreaService serviceAreaService,
            ISumbissionRowRepository submissionRowRepo, IRockfallReportRepository rockfallReportRepo, IFieldValidatorService validator, 
            IEmailService emailService, IConfiguration config,
            ISpatialService spatialService, ILookupCodeService lookupService)
            : base(unitOfWork, statusService, submissionRepo, serviceAreaService, submissionRowRepo, emailService, logger, config, validator, spatialService, lookupService)
        {
            _logger = logger;
            _rockfallReportRepo = rockfallReportRepo;
            _thresholdSpLevel = GetDefaultThresholdSpLevel(Reports.Rockfall);
        }

        /// <summary>
        /// Returns if it can continue to the next submission or not. 
        /// When it encounters a concurrency issue - when there are more than one job for the same submission, 
        /// one of them must stop and the return value indicates whether to stop or not.
        /// </summary>
        /// <param name="submissionDto"></param>
        /// <returns></returns>
        public override async Task<bool> ProcessSubmission(SubmissionDto submissionDto)
        {
            var errors = new Dictionary<string, List<string>>();

            if (!await SetSubmissionAsync(submissionDto))
                return false;

            var (untypedRows, headers) = ParseRowsUnTyped(errors);

            //text after duplicate lines are removed. Will be used for importing to typed DTO.
            var (rowCount, text) = await SetRowIdAndRemoveDuplicate(untypedRows, headers);

            if (rowCount == 0)
            {
                errors.AddItem("File", "No new records were found in the file; all records were already processed in the past submission.");
                _submission.ErrorDetail = errors.GetErrorDetail();
                _submission.SubmissionStatusId = _statusService.FileDuplicate;
                await CommitAndSendEmailAsync();
                return true;
            }

            foreach (var untypedRow in untypedRows)
            {
                errors = new Dictionary<string, List<string>>();

                var submissionRow = _submissionRows[(decimal)untypedRow.RowNum];

                submissionRow.RowStatusId = _statusService.RowSuccess; //set the initial row status as success 
                untypedRow.HighwayUnique = untypedRow.HighwayUnique.ToTrimAndUppercase();
                
                //rockfall requires Y/N fields to be set to Uppercase, see HMCR-643
                untypedRow.HeavyPrecip = untypedRow.HeavyPrecip.ToTrimAndUppercase();
                untypedRow.FreezeThaw = untypedRow.FreezeThaw.ToTrimAndUppercase();
                untypedRow.DitchSnowIce = untypedRow.DitchSnowIce.ToTrimAndUppercase();
                untypedRow.VehicleDamage = untypedRow.VehicleDamage.ToTrimAndUppercase();

                var entityName = GetValidationEntityName(untypedRow);

                _validator.Validate(entityName, untypedRow, errors);

                if (errors.Count > 0)
                {
                    SetErrorDetail(submissionRow, errors, _statusService.FileBasicError);
                }
            }

            var typedRows = new List<RockfallReportTyped>();

            if (_submission.SubmissionStatusId == _statusService.FileInProgress)
            {
                var (rowNum, rows) = ParseRowsTyped(text, errors);

                if (rowNum != 0)
                {
                    var submissionRow = _submissionRows[rowNum];
                    SetErrorDetail(submissionRow, errors, _statusService.FileConflictionError);
                    await CommitAndSendEmailAsync();
                    return true;
                }

                typedRows = rows;

                CopyCalculatedFieldsFormUntypedRow(typedRows, untypedRows);

                PerformAdditionalValidation(typedRows);
            }

            if (_submission.SubmissionStatusId != _statusService.FileInProgress)
            {
                await CommitAndSendEmailAsync();
                return true;
            }

            var rockfallReports = PerformSpatialValidationAndConversionBatchAsync(typedRows);

            _logger.LogInformation($"{_methodLogHeader} PerformSpatialValidationAndConversionAsync 100%");

            if (_submission.SubmissionStatusId != _statusService.FileInProgress)
            {
                await CommitAndSendEmailAsync();
                return true;
            }

            _submission.SubmissionStatusId = _statusService.FileSuccess;

            await foreach (var entity in _rockfallReportRepo.SaveRockfallReportAsnyc(_submission, rockfallReports)) { }

            await CommitAndSendEmailAsync();

            return true;
        }

        private void PerformAdditionalValidation(List<RockfallReportTyped> typedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var errors = new Dictionary<string, List<string>>();
                var submissionRow = _submissionRows[(decimal)typedRow.RowNum];

                if (typedRow.StartOffset != null && typedRow.EndOffset < typedRow.StartOffset)
                {
                    errors.AddItem(Fields.StartOffset, "Start Offset cannot be greater than End Offset");
                }

                if (typedRow.DitchVolume == DitchVolume.Threshold)
                {
                    _validator.Validate(Entities.RockfallReportOtherDitchVolume, Fields.OtherDitchVolume, typedRow.OtherDitchVolume, errors);
                }

                if (typedRow.TravelledLanesVolume == DitchVolume.Threshold)
                {
                    _validator.Validate(Entities.RockfallReportOtherTravelledLanesVolume, Fields.OtherTravelledLanesVolume, typedRow.OtherTravelledLanesVolume, errors);
                }

                if (typedRow.ReportDate != null && typedRow.ReportDate > DateTime.Now)
                {
                    errors.AddItem(Fields.ReportDate, "Cannot be a future date.");
                }

                if (typedRow.EstimatedRockfallDate != null && typedRow.EstimatedRockfallDate > DateTime.Now)
                {
                    errors.AddItem(Fields.EstimatedRockfallDate, "Report Date cannot be a future date.");
                }

                ValidateHighwayUniqueAgainstServiceArea(typedRow.HighwayUnique, errors);

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
                    SetErrorDetail(submissionRow, errors, _statusService.FileConflictionError);
                }
            }
        }

        private void CopyCalculatedFieldsFormUntypedRow(List<RockfallReportTyped> typedRows, List<RockfallReportCsvDto> untypedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var untypedRow = untypedRows.First(x => x.RowNum == typedRow.RowNum);
                typedRow.SpatialData = untypedRow.SpatialData;
                typedRow.RowId = untypedRow.RowId;
            }
        }

        private List<RockfallReportGeometry> PerformSpatialValidationAndConversionBatchAsync(List<RockfallReportTyped> typedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader, $"Total Record: {typedRows.Count}");

            //grouping the rows
            var groups = new List<List<RockfallReportTyped>>();
            var currentGroup = new List<RockfallReportTyped>();

            var count = 0;
            foreach (var typedRow in typedRows)
            {
                currentGroup.Add(typedRow);
                count++;

                if (count % 10 == 0)
                {
                    groups.Add(currentGroup);
                    currentGroup = new List<RockfallReportTyped>();
                }
            }

            if (currentGroup.Count > 0)
            {
                groups.Add(currentGroup);
            }

            var geometries = new ConcurrentBag<RockfallReportGeometry>();
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

        private async Task<RockfallReportGeometry> PerformSpatialValidationAndConversionAsync(RockfallReportTyped typedRow)
        {
            var submissionRow = _submissionRows[(decimal)typedRow.RowNum];
            var rockfallReport = new RockfallReportGeometry(typedRow, null);

            if (typedRow.SpatialData == SpatialData.Gps)
            {
                await PerformSpatialGpsValidation(rockfallReport, submissionRow);

                SetVarianceWarningDetail(submissionRow, typedRow.HighwayUnique,
                    GetGpsString(typedRow.StartLatitude, typedRow.StartLongitude),
                    GetGpsString(typedRow.EndLatitude, typedRow.EndLongitude),
                    _thresholdSpLevel);
            }
            else if (typedRow.SpatialData == SpatialData.Lrs)
            {
                await PerformSpatialLrsValidation(rockfallReport, submissionRow);

                SetVarianceWarningDetail(submissionRow, typedRow.HighwayUnique,
                    GetOffsetString(typedRow.StartOffset),
                    GetOffsetString(typedRow.EndOffset),
                    _thresholdSpLevel);
            }

            return rockfallReport;
        }

        private bool IsPoint(RockfallReportTyped typedRow)
        {
            if (typedRow.SpatialData == SpatialData.Gps)
            {
                //not necessary because they are mandatory
                if (typedRow.EndLongitude == null || typedRow.EndLatitude == null)
                    return true;

                if (typedRow.StartLongitude == typedRow.EndLongitude && typedRow.StartLatitude == typedRow.EndLatitude)
                    return true;
            }
            else
            {
                //not necessary because they are mandatory
                if (typedRow.EndOffset == null)
                    return true;

                if (typedRow.StartOffset == typedRow.EndOffset)
                    return true;
            }

            return false;
        }

        private async Task PerformSpatialGpsValidation(RockfallReportGeometry rockfallReport, HmrSubmissionRow submissionRow)
        {
            var errors = new Dictionary<string, List<string>>();
            var typedRow = rockfallReport.RockfallReportTyped;

            var start = new Chris.Models.Point((decimal)typedRow.StartLongitude, (decimal)typedRow.StartLatitude);

            if (IsPoint(typedRow))
            {
                var result = await _spatialService.ValidateGpsPointAsync(start, typedRow.HighwayUnique, Fields.HighwayUnique, _thresholdSpLevel, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors, _statusService.FileLocationError);
                }
                else if (result.result == SpValidationResult.Success)
                {
                    typedRow.HighwayUniqueLength = result.rfiSegment.Length;
                    typedRow.HighwayUniqueName = result.rfiSegment.Descr;

                    typedRow.StartOffset = result.lrsResult.Offset;
                    typedRow.EndOffset = typedRow.EndOffset;
                    rockfallReport.Geometry = _geometryFactory.CreatePoint(result.lrsResult.SnappedPoint.ToTopologyCoordinate());
                    submissionRow.StartVariance = result.lrsResult.Variance;
                }
            }
            else
            {
                var end = new Chris.Models.Point((decimal)typedRow.EndLongitude, (decimal)typedRow.EndLatitude);
                var result = await _spatialService.ValidateGpsLineAsync(start, end, typedRow.HighwayUnique, Fields.HighwayUnique, _thresholdSpLevel, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors, _statusService.FileLocationError);
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
                    typedRow.WorkLength = (typedRow.WorkLength < 0) ? typedRow.WorkLength * -1 : typedRow.WorkLength;

                    if (result.lines.Count == 1)
                    {
                        if (result.lines[0].ToTopologyCoordinates().Length >= 2)
                        {
                            rockfallReport.Geometry = _geometryFactory.CreateLineString(result.lines[0].ToTopologyCoordinates());
                        }
                        else if (result.lines[0].ToTopologyCoordinates().Length == 1)
                        {
                            _logger.LogInformation($"[Hangfire] Row [{typedRow.RowNum}] [Original: Start[{typedRow.StartLongitude}/{typedRow.StartLatitude}]"
                                + $" End[{typedRow.EndLongitude}/{typedRow.EndLatitude}] were converted to a point [{result.lines[0].Points[0].Longitude}/{result.lines[0].Points[0].Latitude}]");

                            rockfallReport.Geometry = _geometryFactory.CreatePoint(result.lines[0].ToTopologyCoordinates()[0]);
                        }
                    }
                    else if (result.lines.Count > 1)
                    {
                        var lineStrings = new List<LineString>();
                        foreach (var line in result.lines)
                        {
                            lineStrings.Add(_geometryFactory.CreateLineString(line.ToTopologyCoordinates()));
                        }

                        rockfallReport.Geometry = _geometryFactory.CreateMultiLineString(lineStrings.ToArray());
                    }
                }
            }
        }

        private async Task PerformSpatialLrsValidation(RockfallReportGeometry rockfallReport, HmrSubmissionRow submissionRow)
        {
            var errors = new Dictionary<string, List<string>>();
            var typedRow = rockfallReport.RockfallReportTyped;

            //remeber that feature type line/point has been replaced either line or point in PerformGpsEitherLineOrPointValidation().
            if (IsPoint(typedRow))
            {
                var result = await _spatialService.ValidateLrsPointAsync((decimal)typedRow.StartOffset, typedRow.HighwayUnique, Fields.HighwayUnique, _thresholdSpLevel, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors, _statusService.FileLocationError);
                }
                else if (result.result == SpValidationResult.Success)
                {
                    typedRow.HighwayUniqueLength = result.rfiSegment.Length;
                    typedRow.HighwayUniqueName = result.rfiSegment.Descr;

                    typedRow.StartLongitude = result.point.Longitude;
                    typedRow.StartLatitude = result.point.Latitude;
                    typedRow.EndLongitude = typedRow.StartLongitude;
                    typedRow.EndLatitude = typedRow.StartLatitude;
                    rockfallReport.Geometry = _geometryFactory.CreatePoint(result.point.ToTopologyCoordinate());
                    submissionRow.StartVariance = typedRow.StartOffset - result.snappedOffset;
                    submissionRow.EndVariance = submissionRow.StartVariance;
                }
            }
            else
            {
                var result = await _spatialService.ValidateLrsLineAsync((decimal)typedRow.StartOffset, (decimal)typedRow.EndOffset, typedRow.HighwayUnique, Fields.HighwayUnique, _thresholdSpLevel, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors, _statusService.FileLocationError);
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
                    typedRow.WorkLength = (typedRow.WorkLength < 0) ? typedRow.WorkLength * -1 : typedRow.WorkLength;

                    if (result.lines.Count == 1)
                    {
                        if (result.lines[0].ToTopologyCoordinates().Length >= 2)
                        {
                            rockfallReport.Geometry = _geometryFactory.CreateLineString(result.lines[0].ToTopologyCoordinates());
                        }
                        else if (result.lines[0].ToTopologyCoordinates().Length == 1)
                        {
                            _logger.LogInformation($"[Hangfire] Row [{typedRow.RowNum}] [Original: Start[{typedRow.StartOffset}]"
                                + $" End[{typedRow.EndOffset}] were converted to a Start[{result.snappedStartOffset}] End[{result.snappedEndOffset}]");

                            rockfallReport.Geometry = _geometryFactory.CreatePoint(result.lines[0].ToTopologyCoordinates()[0]);
                        }
                    }
                    else if (result.lines.Count > 1)
                    {
                        var lineStrings = new List<LineString>();
                        foreach (var line in result.lines)
                        {
                            lineStrings.Add(_geometryFactory.CreateLineString(line.ToTopologyCoordinates()));
                        }

                        rockfallReport.Geometry = _geometryFactory.CreateMultiLineString(lineStrings.ToArray());
                    }
                }
            }
        }

        private string GetValidationEntityName(RockfallReportCsvDto untypedRow)
        {
            string entityName;
            if (untypedRow.StartLatitude.IsEmpty() || untypedRow.StartLongitude.IsEmpty() &&
                !(untypedRow.StartOffset.IsEmpty() || untypedRow.EndOffset.IsEmpty()))
            {
                entityName = Entities.RockfallReportLrs;
                untypedRow.SpatialData = SpatialData.Lrs;
            }
            else
            {
                entityName = Entities.RockfallReportGps;
                untypedRow.SpatialData = SpatialData.Gps;
            }

            return entityName;
        }

        private (List<RockfallReportCsvDto> untypedRows, string headers) ParseRowsUnTyped(Dictionary<string, List<string>> errors)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            using TextReader textReader = new StreamReader(new MemoryStream(_submission.DigitalRepresentation), Encoding.UTF8);
            using var csv = new CsvReader(textReader, CultureInfo.InvariantCulture);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<RockfallReportCsvDtoMap>();

            var rows = GetRecords(csv);

            return (rows, string.Join(',', csv.Context.HeaderRecord).Replace("\"", ""));
        }

        private List<RockfallReportCsvDto> GetRecords(CsvReader csv)
        {
            var rows = new List<RockfallReportCsvDto>();

            while (csv.Read())
            {
                RockfallReportCsvDto row = null;

                try
                {
                    row = csv.GetRecord<RockfallReportCsvDto>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    throw;
                }

                row.RowNum = csv.Context.Row;
                row.ServiceArea = _serviceArea.ConvertToServiceAreaString(row.ServiceArea);
                rows.Add(row);
            }

            return rows;
        }

        private (decimal rowNum, List<RockfallReportTyped> rows) ParseRowsTyped(string text, Dictionary<string, List<string>> errors)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<RockfallReportDtoMap>();

            var rows = new List<RockfallReportTyped>();
            var rowNum = 0M;

            while (csv.Read())
            {
                try
                {
                    var row = csv.GetRecord<RockfallReportTyped>();
                    row.HighwayUnique = row.HighwayUnique.ToTrimAndUppercase();
                    //rockfall requires Y/N fields to be set to Uppercase, see HMCR-643
                    row.HeavyPrecip = row.HeavyPrecip.ToTrimAndUppercase();
                    row.FreezeThaw = row.FreezeThaw.ToTrimAndUppercase();
                    row.DitchSnowIce = row.DitchSnowIce.ToTrimAndUppercase();
                    row.VehicleDamage = row.VehicleDamage.ToTrimAndUppercase();

                    rows.Add(row);
                    rowNum = (decimal)row.RowNum;
                    row.ServiceArea = _serviceArea.ConvertToServiceAreaNumber(row.ServiceArea);
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
