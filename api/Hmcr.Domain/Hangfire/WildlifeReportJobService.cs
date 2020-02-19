using CsvHelper;
using Hmcr.Data.Database;
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Hmcr.Domain.Hangfire
{
    public interface IWildlifeReportJobService
    {
        Task<bool> ProcessSubmission(SubmissionDto submission);
    }

    public class WildlifeReportJobService : ReportJobServiceBase, IWildlifeReportJobService
    {
        protected IFieldValidatorService _validator;
        private IWildlifeReportRepository _wildlifeReportRepo;

        public WildlifeReportJobService(IUnitOfWork unitOfWork, ILogger<IWildlifeReportJobService> logger,
            ISubmissionStatusRepository statusRepo, ISubmissionObjectRepository submissionRepo,
            ISumbissionRowRepository submissionRowRepo, IWildlifeReportRepository wildlifeReportRepo, IFieldValidatorService validator, 
            IEmailService emailService, IConfiguration config, EmailBody emailBody, IFeebackMessageRepository feedbackRepo)
             : base(unitOfWork, statusRepo, submissionRepo, submissionRowRepo, emailService, logger, config, emailBody, feedbackRepo)
        {
            _logger = logger;
            _wildlifeReportRepo = wildlifeReportRepo;
            _validator = validator;
        }

        /// <summary>
        /// Returns if it can continue to the next submission or not. 
        /// When it encounters a concurrency issue - when there are more than one job for the same submission, 
        /// one of them must stop and the return value indicates whether to stop or not.
        /// </summary>
        /// <param name="submissionDto"></param>
        /// <returns></returns>
        public async Task<bool> ProcessSubmission(SubmissionDto submissionDto)
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

                var entityName = GetValidationEntityName(untypedRow);

                _validator.Validate(entityName, untypedRow, errors);

                if (errors.Count > 0)
                {
                    SetErrorDetail(submissionRow, errors);
                }
            }

            var typedRows = new List<WildlifeReportDto>();

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
            }
            else
            {
                _submission.SubmissionStatusId = _successFileStatusId;

                _wildlifeReportRepo.SaveWildlifeReport(_submission, typedRows);

                await CommitAndSendEmail();
            }

            return true;
        }

        private async Task PerformAdditionalValidationAsync(List<WildlifeReportDto> typedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var errors = new Dictionary<string, List<string>>();
                var submissionRow = await _submissionRowRepo.GetSubmissionRowByRowNum(_submission.SubmissionObjectId, (decimal)typedRow.RowNum);

                //Geo-spatial Validation here
                if (typedRow.AccidentDate != null && typedRow.AccidentDate > DateTime.Now)
                {
                    errors.AddItem(Fields.AccidentDate, "Cannot be a future date.");
                }

                if (errors.Count > 0)
                {
                    SetErrorDetail(submissionRow, errors);
                }
            }
        }

        private void CopyCalculatedFieldsFormUntypedRow(List<WildlifeReportDto> typedRows, List<WildlifeReportCsvDto> untypedRows)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            foreach (var typedRow in typedRows)
            {
                var untypedRow = untypedRows.First(x => x.RowNum == typedRow.RowNum);
                typedRow.SpatialData = untypedRow.SpatialData;
            }
        }

        private string GetValidationEntityName(WildlifeReportCsvDto untypedRow)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader, $"RowNum: {untypedRow.RowNum}");

            var entityName = "";
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
            using var csv = new CsvReader(stringReader);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WildlifeReportCsvDtoMap>();

            var rows = csv.GetRecords<WildlifeReportCsvDto>().ToList();
            for (var i = 0; i < rows.Count; i++)
            {
                rows[i].RowNum = i + 1;
            }

            return (rows, GetHeader(text));
        }

        private (decimal rowNum, List<WildlifeReportDto> rows) ParseRowsTyped(string text, Dictionary<string, List<string>> errors)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            using var stringReader = new StringReader(text);
            using var csv = new CsvReader(stringReader);

            CsvHelperUtils.Config(errors, csv, false);
            csv.Configuration.RegisterClassMap<WildlifeReportDtoMap>();

            var rows = new List<WildlifeReportDto>();
            var rowNum = 0M;

            while (csv.Read())
            {
                try
                {
                    var row = csv.GetRecord<WildlifeReportDto>();
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
