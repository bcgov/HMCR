using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos.FeedbackMessage;
using Hmcr.Model.Utils;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IEmailService
    {
        Task<bool> SendStatusEmailAsync(decimal submissionObjectId, FeedbackMessageUpdateDto feedbackMessage = null);
    }

    public class EmailService : IEmailService
    {
        private IConfiguration _config;

        public string SenderName { get; }
        public string SenderAddress { get; }
        public string Thumbprint { get; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; }

        private IUserRepository _userRepo;
        private ILogger _logger;
        private ISubmissionObjectRepository _submissionRepo;
        private EmailBody _emailBody;
        private IFeebackMessageRepository _feedbackRepo;
        private IUnitOfWork _unitOfWork;

        public EmailService(IConfiguration config, IUserRepository userRepo, ILogger<EmailService> logger, ISubmissionObjectRepository submissionRepo, EmailBody emailBody, 
            IFeebackMessageRepository feedbackRepo, IUnitOfWork unitOfWork)
        {
            _config = config;

            SenderName = config.GetValue<string>("Smtp:SenderName");
            SenderAddress = config.GetValue<string>("Smtp:SenderAddress");
            Thumbprint = config.GetValue<string>("Smtp:Thumbprint");
            SmtpServer = config.GetValue<string>("Smtp:Server");
            SmtpPort = config.GetValue<int>("Smtp:Port");

            _userRepo = userRepo;
            _logger = logger;
            _submissionRepo = submissionRepo;
            _emailBody = emailBody;
            _feedbackRepo = feedbackRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> SendStatusEmailAsync(decimal submissionObjectId, FeedbackMessageUpdateDto feedbackMessage = null)
        {
            var submissionInfo = await _submissionRepo.GetSubmissionInfoForEmailAsync(submissionObjectId);
            submissionInfo.SubmissionDate = DateUtils.ConvertUtcToPacificTime(submissionInfo.SubmissionDate);

            var resultUrl = string.Format(_config.GetValue<string>("Smtp:SubmissionResult"), submissionInfo.ServiceAreaNumber, submissionObjectId);

            var env = _config.GetEnvironment();
            var environment = env == HmcrEnvironments.Prod ? " " : $" [{env}] ";
            var result = submissionInfo.Success ? "SUCCESS" : "ERROR";
            var subject = $"HMCR{environment}report submission({submissionObjectId}) result - {result}";

            var htmlBodyTemplate = submissionInfo.Success ? _emailBody.SuccessHtmlBody() : _emailBody.ErrorHtmlBody(submissionInfo);
            var htmlBody = string.Format(htmlBodyTemplate,
                submissionInfo.FileName, submissionInfo.FileType, submissionInfo.ServiceAreaNumber, submissionInfo.SubmissionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                submissionObjectId, submissionInfo.NumOfRecords, submissionInfo.NumOfDuplicateRecords, submissionInfo.NumOfReplacedRecords,
                submissionInfo.NumOfErrorRecords, submissionInfo.NumOfWarningRecords, resultUrl);

            var textBody = htmlBody.HtmlToPlainText();

            var isSent = true;
            var isError = !submissionInfo.Success;
            var errorText = "";

            try
            {
                SendEmailToUsersInServiceArea(submissionInfo.ServiceAreaNumber, subject, htmlBody, textBody);
            }
            catch (Exception ex)
            {
                isSent = false;
                errorText = ex.Message;

                _logger.LogError($"Email for the submission {submissionObjectId} failed.");
                _logger.LogError(ex.ToString());
            }

            if (feedbackMessage == null)
            {
                var feedback = new FeedbackMessageDto
                {
                    SubmissionObjectId = submissionObjectId,
                    CommunicationSubject = subject,
                    CommunicationText = htmlBody,
                    CommunicationDate = DateTime.UtcNow,
                    IsSent = isSent,
                    IsError = isError,
                    SendErrorText = errorText
                };

                await _feedbackRepo.CreateFeedbackMessageAsync(feedback);
            }
            else
            {
                feedbackMessage.SubmissionObjectId = submissionObjectId;
                feedbackMessage.CommunicationSubject = subject;
                feedbackMessage.CommunicationText = htmlBody;
                feedbackMessage.CommunicationDate = DateTime.UtcNow;
                feedbackMessage.IsSent = isSent;
                feedbackMessage.IsError = isError;
                feedbackMessage.SendErrorText = errorText;

                await _feedbackRepo.UpdateFeedbackMessageAsync(feedbackMessage);
            }

            await _unitOfWork.CommitAsync();

            var finished = isSent ? "Finished" : "Failed";
            var sending = feedbackMessage == null ? "sending" : "resending";

            _logger.LogInformation($"[Hangfire] {finished} {sending} email for submission {submissionObjectId}", submissionObjectId);

            return isSent;
        }

        private void SendEmailToUsersInServiceArea(decimal serviceAreaNumber, string subject, string htmlBody, string textBody)
        {
            var recipients = new List<MailboxAddress>();

            foreach(var user in _userRepo.GetActiveUsersByServiceAreaNumber(serviceAreaNumber))
            {
                if (user.Email.IsNotEmpty())
                    recipients.Add(new MailboxAddress(user.Email));
            }

            SendEmail(recipients, subject, htmlBody, textBody);
        }

        private void SendEmail(List<MailboxAddress> recipients, string subject, string htmlBody, string textBody)
        {
            if (recipients.Count == 0)
            {
                throw new Exception("Email error - no recepients");
            }

            var message = new MimeMessage();

            var from = new MailboxAddress(SenderName, SenderAddress);
            message.From.Add(from);

            foreach (var address in recipients)
            {
                message.Bcc.Add(address);
            }

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlBody;
            bodyBuilder.TextBody = textBody;

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                if (sslPolicyErrors == SslPolicyErrors.None)
                    return true;

                if (chain.ChainElements == null)
                    return false;

                //foreach (var chainCertificate in chain.ChainElements)
                //{
                //    if (chainCertificate.Certificate == null)
                //        continue;

                //    if (chainCertificate.Certificate.Thumbprint == Thumbprint)
                //        return true;
                //}

                //Console.WriteLine("Unable to validate certificate chain.");
                //return false;

                return true;
            };

            client.Connect(SmtpServer, SmtpPort, SecureSocketOptions.Auto);
            client.Send(message);
            client.Disconnect(true);
        }

    }
}
