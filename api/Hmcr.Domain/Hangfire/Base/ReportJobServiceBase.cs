using CsvHelper;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Data.Repositories.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.FeedbackMessage;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Hangfire.Base
{
    public class ReportJobServiceBase
    {
        protected IUnitOfWork _unitOfWork;
        protected ISubmissionStatusService _statusService;
        protected ISubmissionObjectRepository _submissionRepo;
        protected ISumbissionRowRepository _submissionRowRepo;
        private IServiceAreaService _serviceAreaService;
        protected ILogger _logger;
        protected IFieldValidatorService _validator;
        protected ISpatialService _spatialService;
        protected GeometryFactory _geometryFactory;

        protected IEmailService _emailService;
        protected IConfiguration _config;
        protected ILookupCodeService _lookupService;

        protected HmrSubmissionObject _submission;
        protected Dictionary<decimal, HmrSubmissionRow> _submissionRows;

        protected ServiceAreaNumberDto _serviceArea;

        protected bool _enableMethodLog;
        protected string _methodLogHeader;


        public ReportJobServiceBase(IUnitOfWork unitOfWork,
            ISubmissionStatusService statusService, ISubmissionObjectRepository submissionRepo, IServiceAreaService serviceAreaService,
            ISumbissionRowRepository submissionRowRepo, IEmailService emailService, ILogger logger, IConfiguration config, IFieldValidatorService validator,
            ISpatialService spatialService, ILookupCodeService lookupService)
        {
            _unitOfWork = unitOfWork;
            _statusService = statusService;
            _submissionRepo = submissionRepo;
            _submissionRowRepo = submissionRowRepo;
            _serviceAreaService = serviceAreaService;
            _emailService = emailService;
            _logger = logger;
            _config = config;
            _lookupService = lookupService;
            _validator = validator;
            _spatialService = spatialService;
            _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            _submissionRows = new Dictionary<decimal, HmrSubmissionRow>();
        }

        public async Task<bool> ProcessSubmissionMain(SubmissionDto submissionDto) 
        {
            try
            {
                return await ProcessSubmission(submissionDto);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());

                if (_submission != null)
                {
                    ((HmcrRepositoryBase<HmrSubmissionObject>)_submissionRepo).RollBackEntities();

                    _submission.ErrorDetail = $"{FileError.UnknownException}: {ex.Message}";
                    _submission.SubmissionStatusId = _statusService.FileUnexpectedError;
                    await CommitAndSendEmailAsync();
                }
            }

            return true;
        }

        public void UpdateSubmissionStatus(decimal submissionStatusId)
        {
            _submission.SubmissionStatusId = submissionStatusId;
            _unitOfWork.Commit();
        } 

        public virtual Task<bool> ProcessSubmission(SubmissionDto submissionDto)
        {
            throw new NotImplementedException();
        }

        protected async Task<bool> SetSubmissionAsync(SubmissionDto submissionDto)
        {
            _logger.LogInformation("[Hangfire] Starting submission {submissionObjectId}", (long)submissionDto.SubmissionObjectId);

            _submission = await _submissionRepo.GetSubmissionObjecForBackgroundJobAsync(submissionDto.SubmissionObjectId);
            _submission.SubmissionStatusId = _statusService.FileInProgress;
            _unitOfWork.Commit();

            _submissionRows = new Dictionary<decimal, HmrSubmissionRow>();

            _methodLogHeader = $"[Hangfire] Submission ({_submission.SubmissionObjectId}): ";
            _enableMethodLog = _config.GetValue<string>("DISABLE_METHOD_LOGGER") != "Y"; //enabled by default

            _serviceArea = await _serviceAreaService.GetServiceAreaByServiceAreaNumberAsyc(_submission.ServiceAreaNumber);

            return true;
        }

        protected async Task<(int rowCount, string text)> SetRowIdAndRemoveDuplicate<T>(List<T> untypedRows, string headers) where T : IReportCsvDto
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            headers = $"{Fields.RowNum}," + headers;
            var text = new StringBuilder();
            text.AppendLine(headers);

            var rowCount = 0;
            for (int i = untypedRows.Count - 1; i >= 0; i--)
            {
                var untypedRow = untypedRows[i];
                var rowNum = (decimal)untypedRow.RowNum;
                var entity = await _submissionRowRepo.GetSubmissionRowByRowNumAsync(_submission.SubmissionObjectId, rowNum);

                if (entity.RowStatusId == _statusService.RowDuplicate)
                {
                    untypedRows.RemoveAt(i);
                    continue;
                }

                _submissionRows.Add(rowNum, entity);

                rowCount++;
                text.AppendLine($"{untypedRow.RowNum},{entity.RowValue}");
                untypedRow.RowId = entity.RowId;
            }

            return (rowCount, text.ToString());
        }

        protected void SetErrorDetail(HmrSubmissionRow submissionRow, Dictionary<string, List<string>> errors, decimal submissionStatusId)
        {
            if (submissionRow != null)
            {
                submissionRow.RowStatusId = _statusService.RowError;
                submissionRow.ErrorDetail = errors.GetErrorDetail();
                _submission.ErrorDetail = FileError.ReferToRowErrors;
                _submission.SubmissionStatusId = submissionStatusId;
            }
            else
            {
                _submission.ErrorDetail = errors.GetErrorDetail();
                _submission.SubmissionStatusId = submissionStatusId;
            }
        }

        protected void SetWarningDetail(HmrSubmissionRow submissionRow, Dictionary<string, List<string>> warnings)
        {
            //warning can only be set for a row
            if (submissionRow != null)
            {
                if (submissionRow.WarningDetail != null)
                {
                    var newWarnings = System.Text.Json.JsonSerializer.Deserialize<MessageDetail>(submissionRow.WarningDetail);
                    foreach (var warning in warnings) 
                    {
                        newWarnings.FieldMessages.Add(new FieldMessage
                        {
                            Field = warning.Key.WordToWords(),
                            Messages = warning.Value
                        });
                    }
                    submissionRow.WarningDetail = newWarnings.ToString();
                } else
                {
                    //rows don't need to error when a warning is thrown, it's still a success but with a message.
                    submissionRow.WarningDetail = warnings.GetWarningDetail();
                }
            }
        }

        protected bool HasWarningSet()
        {
            bool hasWarningSet = false;

            foreach (var row in _submission.HmrSubmissionRows)
            {
                if (row.WarningDetail != null)
                {
                    hasWarningSet = true;
                    break;
                }
            }

            return hasWarningSet;
        }

        protected async Task CommitAndSendEmailAsync()
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            _unitOfWork.Commit();

            await _emailService.SendStatusEmailAsync(_submission.SubmissionObjectId);

            _logger.LogInformation("[Hangfire] Finishing submission {submissionObjectId}", _submission.SubmissionObjectId);
        }

        protected void LogRowParseException(decimal rowNum, string exception, ReadingContext context)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            _logger.LogError($"Exception while parsing the line [{rowNum}]");
            _logger.LogError(string.Join(',', context.HeaderRecord));
            _logger.LogError(context.RawRecord);
        }

        protected void ValidateHighwayUniqueAgainstServiceArea(string highwayUnique, Dictionary<string, List<string>> errors)
        {
            if (highwayUnique.IsEmpty())
                return;

            if (highwayUnique.Length < 2)
            {
                errors.AddItem(Fields.HighwayUnique, $"Highway Unique [{highwayUnique}] does not belong to the service area [{_serviceArea.Name}]");
                return;
            }

            var huPrefix = highwayUnique.Substring(0, 2);

            if (!_serviceArea.HighwayUniquePrefixes.Contains(huPrefix))
            {
                errors.AddItem(Fields.HighwayUnique, $"Highway Unique [{highwayUnique}] does not belong to the service area [{_serviceArea.Name}]");
            }            
        }

        protected decimal GetRowNum(string row)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            var cols = row.Split(',');
            if (!decimal.TryParse(cols[0], out decimal rowNum))
            {
                _logger.LogError($"Parsing cols[0] to rowNum failed. Using rowNum 1 instead for logging.");
                rowNum = 1M;
            }

            return rowNum;
        }

        protected void SetVarianceWarningDetail(HmrSubmissionRow row, string rfiSegment, string start, string end, string thresholdLevel)
        {
            var warnings = new Dictionary<string, List<string>>();
            var threshold = _lookupService.GetThresholdLevel(thresholdLevel);
            var threasholdInKm = threshold.Warning / 1000M;

            row.ErrorSpThreshold = threshold.Error / 1000M;
            row.WarningSpThreshold = threshold.Warning / 1000M;

            if (row.StartVariance != null && row.StartVariance > threasholdInKm)
            {
                //row.WarningDetail = string.Format(RowWarning.VarianceWarning, "Start", start, rfiSegment, threshold.Warning);
                warnings.AddItem("Start GPS Variance", string.Format("{0} {1} is is not on the Highway Unique [{2}] within the warning threshold [{3}] metres",
                    "Start", start, rfiSegment, threshold.Warning));
            }
            else if (row.EndVariance != null && row.EndVariance > threasholdInKm)
            {
                //row.WarningDetail = string.Format(RowWarning.VarianceWarning, "End", end, rfiSegment, threshold.Warning);
                warnings.AddItem("End GPS Variance", string.Format("{0} {1} is is not on the Highway Unique [{2}] within the warning threshold [{3}] metres",
                    "End", end, rfiSegment, threshold.Warning));
            }

            if (warnings.Count > 0)
            {
                SetWarningDetail(row, warnings);
            }
        }

        protected string GetGpsString(decimal? latitude, decimal? longitude)
        {
            return $"GPS position [{latitude},{longitude}]";
        }

        protected string GetOffsetString(decimal? offset)
        {
            return $"Offset [{offset}]";
        }

        protected bool ValidateGpsCoordsRange(decimal? longitude, decimal? latitude)
        {
            //do not validate
            if (longitude == null || latitude == null)
                return true;

            if (latitude > GpsCoords.MinLatitude && latitude < GpsCoords.MaxLatitude && longitude > GpsCoords.MinLongitude && longitude < GpsCoords.MaxLongitude)
                return true;

            return false;
        }

        protected string GetDefaultThresholdSpLevel(string report)
        {
            var level = _config.GetValue<string>($"DefaultThresholdSpLevel:{report}");

            if (level.IsEmpty())
                throw new Exception($"Cannot find the configuration of the default spatial threshold level for {report} report.");

            if (level.ToLowerInvariant() == ThresholdSpLevels.Level1.ToLowerInvariant())
                return ThresholdSpLevels.Level1;

            if (level.ToLowerInvariant() == ThresholdSpLevels.Level2.ToLowerInvariant())
                return ThresholdSpLevels.Level2;

            if (level.ToLowerInvariant() == ThresholdSpLevels.Level3.ToLowerInvariant())
                return ThresholdSpLevels.Level3;

            if (level.ToLowerInvariant() == ThresholdSpLevels.Level4.ToLowerInvariant())
                return ThresholdSpLevels.Level4;

            throw new Exception($"The configured spatial threshold level {level} is not valid.");
        }
    }
}
