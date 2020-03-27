using CsvHelper;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Domain.CsvHelpers;
using Hmcr.Domain.Hangfire.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.WildlifeReport;
using Hmcr.Model.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    public interface IWildlifeReportJobService
    {
        Task<bool> ProcessSubmissionMain(SubmissionDto submission);
    }

    public class WildlifeReportJobService : ReportJobServiceBase, IWildlifeReportJobService
    {
        private IWildlifeReportRepository _wildlifeReportRepo;

        public WildlifeReportJobService(IUnitOfWork unitOfWork, ILogger<IWildlifeReportJobService> logger,
            ISubmissionStatusRepository statusRepo, ISubmissionObjectRepository submissionRepo,
            ISumbissionRowRepository submissionRowRepo, IWildlifeReportRepository wildlifeReportRepo, IFieldValidatorService validator, 
            IEmailService emailService, IConfiguration config, EmailBody emailBody, IFeebackMessageRepository feedbackRepo,
            ISpatialService spatialService)
             : base(unitOfWork, statusRepo, submissionRepo, submissionRowRepo, emailService, logger, config, validator, spatialService, emailBody, feedbackRepo)
        {
            _wildlifeReportRepo = wildlifeReportRepo;
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

            if (!CheckCommonMandatoryHeaders(untypedRows, new WildlifeReportHeaders(), errors))
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

                var entityName = GetValidationEntityName(untypedRow);

                _validator.Validate(entityName, untypedRow, errors);

                if (errors.Count > 0)
                {
                    SetErrorDetail(submissionRow, errors);
                }
            }

            var typedRows = new List<WildlifeReportTyped>();

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

            var wildlifeReports = PerformSpatialValidationAndConversionBatchAsync(typedRows);

            _logger.LogInformation($"{_methodLogHeader} PerformSpatialValidationAndConversionAsync 100%");

            if (_submission.SubmissionStatusId == _errorFileStatusId)
            {
                await CommitAndSendEmailAsync();
                return true;
            }

            _submission.SubmissionStatusId = _successFileStatusId;

            _wildlifeReportRepo.SaveWildlifeReport(_submission, wildlifeReports);

            await CommitAndSendEmailAsync();

            return true;
        }

        private void PerformAdditionalValidation(List<WildlifeReportTyped> typedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var errors = new Dictionary<string, List<string>>();
                var submissionRow = _submissionRows[(decimal)typedRow.RowNum];

                if (typedRow.AccidentDate != null && typedRow.AccidentDate > DateTime.Now)
                {
                    errors.AddItem(Fields.AccidentDate, "Cannot be a future date.");
                }

                if (!ValidateGpsCoordsRange(typedRow.Longitude, typedRow.Latitude))
                {
                    errors.AddItem($"{Fields.Longitude}/{Fields.Latitude}", "Invalid range of GPS coordinates.");
                }

                if (errors.Count > 0)
                {
                    SetErrorDetail(submissionRow, errors);
                }
            }
        }

        private void CopyCalculatedFieldsFormUntypedRow(List<WildlifeReportTyped> typedRows, List<WildlifeReportCsvDto> untypedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var untypedRow = untypedRows.First(x => x.RowNum == typedRow.RowNum);
                typedRow.SpatialData = untypedRow.SpatialData;
                typedRow.RowId = untypedRow.RowId;
            }
        }

        private List<WildlifeReportGeometry> PerformSpatialValidationAndConversionBatchAsync(List<WildlifeReportTyped> typedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader, $"Total Record: {typedRows.Count}");

            //grouping the rows
            var groups = new List<List<WildlifeReportTyped>>();
            var currentGroup = new List<WildlifeReportTyped>();

            var count = 0;
            foreach (var typedRow in typedRows)
            {
                currentGroup.Add(typedRow);
                count++;

                if (count % 10 == 0)
                {
                    groups.Add(currentGroup);
                    currentGroup = new List<WildlifeReportTyped>();
                }
            }

            if (currentGroup.Count > 0)
            {
                groups.Add(currentGroup);
            }

            var geometries = new ConcurrentBag<WildlifeReportGeometry>();
            var progress = 0;

            var step = Math.Round((double)typedRows.Count / 100) * 10;
            step = step == 0 ? 10 : step;

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

        private async Task<WildlifeReportGeometry> PerformSpatialValidationAndConversionAsync(WildlifeReportTyped typedRow)
        {
            var submissionRow = _submissionRows[(decimal)typedRow.RowNum];
            var wildlifeReport = new WildlifeReportGeometry(typedRow, null);

            if (typedRow.SpatialData == SpatialData.Gps)
            {
                await PerformSpatialGpsValidation(wildlifeReport, submissionRow);
            }
            else if (typedRow.SpatialData == SpatialData.Lrs)
            {
                await PerformSpatialLrsValidation(wildlifeReport, submissionRow);
            }

            SetVarianceWarningDetail(submissionRow, typedRow.HighwayUnique);

            return wildlifeReport;
        }

        private async Task PerformSpatialGpsValidation(WildlifeReportGeometry wildlifeReport, HmrSubmissionRow submissionRow)
        {
            var errors = new Dictionary<string, List<string>>();
            var typedRow = wildlifeReport.WildlifeReportTyped;

            var start = new Chris.Models.Point((decimal)typedRow.Longitude, (decimal)typedRow.Latitude);

            var result = await _spatialService.ValidateGpsPointAsync(start, typedRow.HighwayUnique, Fields.HighwayUnique, errors);

            if (result.result == SpValidationResult.Fail)
            {
                SetErrorDetail(submissionRow, errors);
            }
            else if (result.result == SpValidationResult.Success)
            {
                typedRow.HighwayUniqueLength = result.rfiSegment.Length;
                typedRow.HighwayUniqueName = result.rfiSegment.Descr;

                typedRow.Offset = result.lrsResult.Offset;
                wildlifeReport.Geometry = _geometryFactory.CreatePoint(result.lrsResult.SnappedPoint.ToTopologyCoordinate());
                submissionRow.StartVariance = result.lrsResult.Variance;
            }
        }

        private async Task PerformSpatialLrsValidation(WildlifeReportGeometry wildlifeReport, HmrSubmissionRow submissionRow)
        {
            var errors = new Dictionary<string, List<string>>();
            var typedRow = wildlifeReport.WildlifeReportTyped;

            var result = await _spatialService.ValidateLrsPointAsync((decimal)typedRow.Offset, typedRow.HighwayUnique, Fields.HighwayUnique, errors);

            if (result.result == SpValidationResult.Fail)
            {
                SetErrorDetail(submissionRow, errors);
            }
            else if (result.result == SpValidationResult.Success)
            {
                typedRow.HighwayUniqueLength = result.rfiSegment.Length;
                typedRow.HighwayUniqueName = result.rfiSegment.Descr;

                typedRow.Longitude = result.point.Longitude;
                typedRow.Latitude = result.point.Latitude;
                wildlifeReport.Geometry = _geometryFactory.CreatePoint(result.point.ToTopologyCoordinate());
                submissionRow.StartVariance = typedRow.Offset - result.snappedOffset;
            }
        }

        private string GetValidationEntityName(WildlifeReportCsvDto untypedRow)
        {
            string entityName;
            if (untypedRow.Latitude.IsEmpty() || untypedRow.Longitude.IsEmpty())
            {
                entityName = Entities.WildlifeReportLrs;
                untypedRow.SpatialData = SpatialData.Lrs;
            }
            else
            {
                entityName = Entities.WildlifeReportGps;
                untypedRow.SpatialData = SpatialData.Gps;
            }

            return entityName;
        }

        private (List<WildlifeReportCsvDto> untypedRows, string headers) ParseRowsUnTyped(Dictionary<string, List<string>> errors)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            var text = Encoding.UTF8.GetString(_submission.DigitalRepresentation);

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WildlifeReportCsvDtoMap>();

            var rows = csv.GetRecords<WildlifeReportCsvDto>().ToList();
            for (var i = 0; i < rows.Count; i++)
            {
                rows[i].RowNum = i + 2;
            }

            return (rows, GetHeader(text));
        }

        private (decimal rowNum, List<WildlifeReportTyped> rows) ParseRowsTyped(string text, Dictionary<string, List<string>> errors)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WildlifeReportDtoMap>();

            var rows = new List<WildlifeReportTyped>();
            var rowNum = 0M;

            while (csv.Read())
            {
                try
                {
                    var row = csv.GetRecord<WildlifeReportTyped>();
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
