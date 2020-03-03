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
using System;
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

        public RockfallReportJobService(IUnitOfWork unitOfWork, ILogger<IRockfallReportJobService> logger, 
            ISubmissionStatusRepository statusRepo, ISubmissionObjectRepository submissionRepo,
            ISumbissionRowRepository submissionRowRepo, IRockfallReportRepository rockfallReportRepo, IFieldValidatorService validator, 
            IEmailService emailService, IConfiguration config, EmailBody emailBody, IFeebackMessageRepository feedbackRepo,
            ISpatialService spatialService)
            : base(unitOfWork, statusRepo, submissionRepo, submissionRowRepo, emailService, logger, config, validator, spatialService, emailBody, feedbackRepo)
        {
            _logger = logger;
            _rockfallReportRepo = rockfallReportRepo;
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

            await SetMemberVariablesAsync();

            if (!await SetSubmissionAsync(submissionDto))
                return false;

            var (untypedRows, headers) = ParseRowsUnTyped(errors);

            if (!CheckCommonMandatoryHeaders(untypedRows, new RockfallReportHeaders(), errors))
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
                var submissionRow = await _submissionRowRepo.GetSubmissionRowByRowId(untypedRow.RowId);
                submissionRow.RowStatusId = _successRowStatusId; //set the initial row status as success 

                var entityName = GetValidationEntityName(untypedRow);

                _validator.Validate(entityName, untypedRow, errors);

                if (errors.Count > 0)
                {
                    SetErrorDetail(submissionRow, errors);
                }
            }

            var typedRows = new List<RockfallReportTyped>();

            if (_submission.SubmissionStatusId != _errorFileStatusId)
            {
                var (rowNum, rows) = ParseRowsTyped(text, errors);

                if (rowNum != 0)
                {
                    var submissionRow = await _submissionRowRepo.GetSubmissionRowByRowNum(_submission.SubmissionObjectId, rowNum);
                    SetErrorDetail(submissionRow, errors);
                    await CommitAndSendEmailAsync();
                    return true;
                }

                typedRows = rows;

                CopyCalculatedFieldsFormUntypedRow(typedRows, untypedRows);

                await PerformAdditionalValidationAsync(typedRows);
            }

            if (_submission.SubmissionStatusId == _errorFileStatusId)
            {
                await CommitAndSendEmailAsync();
                return true;
            }

            var rockfallReports = new List<RockfallReportGeometry>();

            //Spatial Validation and Conversion
            await foreach (var rockfallReport in PerformSpatialValidationAndConversionAsync(typedRows))
            {
                rockfallReports.Add(rockfallReport);
                _logger.LogInformation($"[Hangfire] Spatial Validation for the row [{rockfallReport.RockfallReportTyped.RowNum}] [{rockfallReport.RockfallReportTyped.SpatialData}]");
            }

            if (_submission.SubmissionStatusId == _errorFileStatusId)
            {
                await CommitAndSendEmailAsync();
                return true;
            }

            _submission.SubmissionStatusId = _successFileStatusId;

            await foreach (var entity in _rockfallReportRepo.SaveRockfallReportAsnyc(_submission, rockfallReports)) { }

            await CommitAndSendEmailAsync();

            return true;
        }

        private async Task PerformAdditionalValidationAsync(List<RockfallReportTyped> typedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var errors = new Dictionary<string, List<string>>();
                var submissionRow = await _submissionRowRepo.GetSubmissionRowByRowNum(_submission.SubmissionObjectId, (decimal)typedRow.RowNum);

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

        private async IAsyncEnumerable<RockfallReportGeometry> PerformSpatialValidationAndConversionAsync(List<RockfallReportTyped> typedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var submissionRow = await _submissionRowRepo.GetSubmissionRowByRowNum(_submission.SubmissionObjectId, (decimal)typedRow.RowNum);
                var rockfallReport = new RockfallReportGeometry(typedRow, null);

                if (typedRow.SpatialData == SpatialData.Gps)
                {
                    await PerformSpatialGpsValidation(rockfallReport, submissionRow);
                }
                else if (typedRow.SpatialData == SpatialData.Lrs)
                {
                    await PerformSpatialLrsValidation(rockfallReport, submissionRow);
                }

                SetVarianceWarningDetail(submissionRow);
                yield return rockfallReport;
            }
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
                var result = await _spatialService.ValidateGpsPointAsync(start, typedRow.HighwayUnique, Fields.HighwayUnique, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors);
                }
                else if (result.result == SpValidationResult.Success)
                {
                    typedRow.StartOffset = result.lrsResult.Offset;
                    typedRow.EndOffset = typedRow.EndOffset;
                    rockfallReport.Geometry = _geometryFactory.CreatePoint(result.lrsResult.SnappedPoint.ToTopologyCoordinate());
                    submissionRow.StartVariance = result.lrsResult.Variance;
                }
            }
            else
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
                    submissionRow.StartVariance = result.startPointResult.Variance;

                    typedRow.EndOffset = result.endPointResult.Offset;
                    submissionRow.EndVariance = result.endPointResult.Variance;

                    if (result.line.ToTopologyCoordinates().Length >= 2)
                    {
                        rockfallReport.Geometry = _geometryFactory.CreateLineString(result.line.ToTopologyCoordinates());
                    }
                    else if (result.line.ToTopologyCoordinates().Length == 1)
                    {
                        _logger.LogInformation($"[Hangfire] Row [{typedRow.RowNum}] [Original: Start[{typedRow.StartLongitude}/{typedRow.StartLatitude}]"
                            + $" End[{typedRow.EndLongitude}/{typedRow.EndLatitude}] were converted to a point [{result.line.Points[0].Longitude}/{result.line.Points[0].Latitude}]");

                        rockfallReport.Geometry = _geometryFactory.CreatePoint(result.line.ToTopologyCoordinates()[0]);
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
                var result = await _spatialService.ValidateLrsPointAsync((decimal)typedRow.StartOffset, typedRow.HighwayUnique, Fields.HighwayUnique, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors);
                }
                else if (result.result == SpValidationResult.Success)
                {
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
                var result = await _spatialService.ValidateLrsLineAsync((decimal)typedRow.StartOffset, (decimal)typedRow.EndOffset, typedRow.HighwayUnique, Fields.HighwayUnique, errors);

                if (result.result == SpValidationResult.Fail)
                {
                    SetErrorDetail(submissionRow, errors);
                }
                else if (result.result == SpValidationResult.Success)
                {
                    typedRow.StartLongitude = result.startPoint.Longitude;
                    typedRow.StartLatitude = result.startPoint.Latitude;
                    submissionRow.StartVariance = typedRow.StartOffset - result.snappedStartOffset;

                    typedRow.EndLongitude = result.endPoint.Longitude;
                    typedRow.EndLatitude = result.endPoint.Latitude;
                    submissionRow.EndVariance = typedRow.EndOffset - result.snappedEndOffset;

                    if (result.line.ToTopologyCoordinates().Length >= 2)
                    {
                        rockfallReport.Geometry = _geometryFactory.CreateLineString(result.line.ToTopologyCoordinates());
                    }
                    else if (result.line.ToTopologyCoordinates().Length == 1)
                    {
                        _logger.LogInformation($"[Hangfire] Row [{typedRow.RowNum}] [Original: Start[{typedRow.StartOffset}]"
                            + $" End[{typedRow.EndOffset}] were converted to a Start[{result.snappedStartOffset}] End[{result.snappedEndOffset}]");

                        rockfallReport.Geometry = _geometryFactory.CreatePoint(result.line.ToTopologyCoordinates()[0]);
                    }
                }
            }
        }

        private string GetValidationEntityName(RockfallReportCsvDto untypedRow)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader, $"RowNum: {untypedRow.RowNum}");

            string entityName;
            if (untypedRow.StartLatitude.IsEmpty() || untypedRow.StartLongitude.IsEmpty())
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

            var text = Encoding.UTF8.GetString(_submission.DigitalRepresentation);

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<RockfallReportCsvDtoMap>();

            var rows = csv.GetRecords<RockfallReportCsvDto>().ToList();
            for (var i = 0; i < rows.Count; i++)
            {
                rows[i].RowNum = i + 2;
            }

            return (rows, GetHeader(text));
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
