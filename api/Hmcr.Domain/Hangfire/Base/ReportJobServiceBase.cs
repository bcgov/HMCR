using CsvHelper;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.FeedbackMessage;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        protected ISubmissionStatusRepository _statusRepo;
        protected ISubmissionObjectRepository _submissionRepo;
        protected ISumbissionRowRepository _submissionRowRepo;
        private IEmailService _emailService;
        protected ILogger _logger;
        private IConfiguration _config;
        private EmailBody _emailBody;
        private IFeebackMessageRepository _feedbackRepo;
        protected decimal _duplicateRowStatusId;
        protected decimal _errorRowStatusId;
        protected decimal _successRowStatusId;
        protected decimal _errorFileStatusId;
        protected decimal _successFileStatusId;
        protected decimal _inProgressRowStatusId;
        protected decimal _duplicateFileStatusId;

        protected HmrSubmissionObject _submission;

        protected bool _enableMethodLog;
        protected string _methodLogHeader;

        public ReportJobServiceBase(IUnitOfWork unitOfWork,
            ISubmissionStatusRepository statusRepo, ISubmissionObjectRepository submissionRepo,
            ISumbissionRowRepository submissionRowRepo, IEmailService emailService, ILogger logger, IConfiguration config,
            EmailBody emailBody, IFeebackMessageRepository feedbackRepo)
        {
            _unitOfWork = unitOfWork;
            _statusRepo = statusRepo;
            _submissionRepo = submissionRepo;
            _submissionRowRepo = submissionRowRepo;
            _emailService = emailService;
            _logger = logger;
            _config = config;
            _emailBody = emailBody;
            _feedbackRepo = feedbackRepo;
        }

        protected async Task SetMemberVariablesAsync()
        {
            var statuses = await _statusRepo.GetActiveStatuses();

            _duplicateRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.DuplicateRow).StatusId;
            _errorRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.RowError).StatusId;
            _successRowStatusId = statuses.First(x => x.StatusType == StatusType.Row && x.StatusCode == RowStatus.Success).StatusId;

            _errorFileStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.DataError).StatusId;
            _successFileStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.Success).StatusId;
            _duplicateFileStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.DuplicateSubmission).StatusId;

            _inProgressRowStatusId = statuses.First(x => x.StatusType == StatusType.File && x.StatusCode == FileStatus.InProgress).StatusId;
        }

        protected async Task<bool> SetSubmissionAsync(SubmissionDto submissionDto)
        {
            _logger.LogInformation("[Hangfire] Starting submission {submissionObjectId}", (long)submissionDto.SubmissionObjectId);

            var success = await _submissionRepo.UpdateSubmissionStatusAsync(submissionDto.SubmissionObjectId, _inProgressRowStatusId, submissionDto.ConcurrencyControlNumber);

            if (!success)
                return false;

            _submission = await _submissionRepo.GetSubmissionObjecForBackgroundJobAsync(submissionDto.SubmissionObjectId);

            _methodLogHeader = $"[Hangfire] Submission ({_submission.SubmissionObjectId}): ";
            _enableMethodLog = _config.GetValue<string>("DISABLE_METHOD_LOGGER") != "Y"; //enabled by default

            return true;
        }

        protected bool CheckCommonMandatoryHeaders<T1, T2>(List<T1> rows, T2 headers, Dictionary<string, List<string>> errors) where T2 : IReportHeaders
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            if (rows.Count == 0) //not possible since it's already validated in the ReportServiceBase.
                throw new Exception("File has no rows.");

            var row = rows[0];

            var fields = typeof(T1).GetProperties();

            foreach (var field in fields)
            {
                if (!headers.CommonMandatoryFields.Any(x => x == field.Name))
                    continue;

                if (field.GetValue(row) == null)
                {
                    errors.AddItem("File", $"Header [{field.Name.WordToWords()}] is missing");
                }
            }

            return errors.Count == 0;
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
                var entity = await _submissionRowRepo.GetSubmissionRowByRowNum(_submission.SubmissionObjectId, (decimal)untypedRow.RowNum);

                if (entity.RowStatusId == _duplicateRowStatusId)
                {
                    untypedRows.RemoveAt(i);
                    continue;
                }

                rowCount++;
                text.AppendLine($"{untypedRow.RowNum},{entity.RowValue}");
                untypedRow.RowId = entity.RowId;
            }

            return (rowCount, text.ToString());
        }

        protected void SetErrorDetail(HmrSubmissionRow submissionRow, Dictionary<string, List<string>> errors)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            submissionRow.RowStatusId = _errorRowStatusId;
            submissionRow.ErrorDetail = errors.GetErrorDetail();
            _submission.ErrorDetail = FileError.ReferToRowErrors;
            _submission.SubmissionStatusId = _errorFileStatusId;
        }
        protected string GetHeader(string text)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            if (text == null)
                return "";

            using var reader = new StringReader(text);
            var header = reader.ReadLine().Replace("\"", "");

            return header ?? "";
        }

        protected async Task CommitAndSendEmail()
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            _unitOfWork.Commit();

            var submissionInfo = await _submissionRepo.GetSubmissionInfoForEmail(_submission.SubmissionObjectId);

            DateTime.SpecifyKind(submissionInfo.SubmissionDate, DateTimeKind.Utc);
            submissionInfo.SubmissionDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(submissionInfo.SubmissionDate, Constants.PacificTimeZone);
            //submissionInfo.SubmissionDate = submissionInfo.SubmissionDate.ToLocalTime();

            var submissionId = _submission.SubmissionObjectId;
            var resultUrl = string.Format(_config.GetValue<string>("SUBMISSION_RESULT"), _submission.ServiceAreaNumber, submissionId);

            var env = _config.GetEnvironment();
            var environment = env == HmcrEnvironments.Prod ? " " : $" [{env}] ";
            var result = submissionInfo.Success ? "SUCCESS" : "ERROR";
            var subject = $"HMCR{environment}report submission({submissionId}) result - {result}";

            var htmlBodyTemplate = submissionInfo.Success ? _emailBody.SuccessHtmlBody : _emailBody.ErrorHtmlBody;
            var htmlBody = string.Format(htmlBodyTemplate, 
                submissionInfo.FileName, submissionInfo.FileType, submissionInfo.ServiceAreaNumber, submissionInfo.SubmissionDate.ToString("yyyy-MM-dd HH:mm:ss"), 
                submissionId, submissionInfo.NumOfRecords, submissionInfo.NumOfErrorRecords, resultUrl);

            var textBody = htmlBody.HtmlToPlainText();

            var isSent = true;
            var isError = false;
            var errorText = "";

            try
            {
                _emailService.SendEmailToUsersInServiceArea(_submission.ServiceAreaNumber, subject, htmlBody, textBody);
            }
            catch (Exception ex)
            {
                isSent = false;
                isError = true;
                errorText = ex.Message;

                _logger.LogError(ex.ToString());
            }

            var feedback = new FeedbackMessageDto
            {
                SubmissionObjectId = _submission.SubmissionObjectId,
                CommunicationSubject = subject,
                CommunicationText = htmlBody,
                CommunicationDate = DateTime.UtcNow,
                IsSent = isSent,
                IsError = isError,
                SendErrorText = errorText
            };

            await _feedbackRepo.CreateFeedbackMessage(feedback);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[Hangfire] Finishing submission {submissionObjectId}", submissionId);
        }

        protected void LogRowParseException(decimal rowNum, string exception, ReadingContext context)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            _logger.LogError($"Exception while parsing the line [{rowNum}]");
            _logger.LogError(string.Join(',', context.HeaderRecord));
            _logger.LogError(context.RawRecord);
        }

        protected decimal GetRowNum(string row)
        {
            MethodLogger.LogEntry(_logger, _enableMethodLog, _methodLogHeader);

            var cols = row.Split(',');
            if (decimal.TryParse(cols[0], out decimal rowNum))
            {
                _logger.LogError($"Parsing cols[0] to rowNum failed. Using rowNum 1 instead for logging.");
                rowNum = 1M;
            }

            return rowNum;
        }
    }
}
