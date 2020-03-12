using Hmcr.Data.Repositories;
using Hmcr.Model.Utils;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Net.Security;

namespace Hmcr.Domain.Services
{
    public interface IEmailService
    {
        void SendEmailToUsersInServiceArea(decimal serviceAreaNumber, string subject, string htmlBody, string textBody);
    }

    public class EmailService : IEmailService
    {
        public string SenderName { get; }
        public string SenderAddress { get; }
        public string Thumbprint { get; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; }

        private IUserRepository _userRepo;

        public EmailService(IConfiguration config, IUserRepository userRepo)
        {
            SenderName = config.GetValue<string>("Smtp:SenderName");
            SenderAddress = config.GetValue<string>("Smtp:SenderAddress");
            Thumbprint = config.GetValue<string>("Smtp:Thumbprint");
            SmtpServer = config.GetValue<string>("Smtp:Server");
            SmtpPort = config.GetValue<int>("Smtp:Port");

            _userRepo = userRepo;
        }

        public void SendEmailToUsersInServiceArea(decimal serviceAreaNumber, string subject, string htmlBody, string textBody)
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

                foreach (var chainCertificate in chain.ChainElements)
                {
                    if (chainCertificate.Certificate == null)
                        continue;

                    if (chainCertificate.Certificate.Thumbprint == Thumbprint)
                        return true;
                }

                Console.WriteLine("Unable to validate certificate chain.");
                return false;
            };

            client.Connect(SmtpServer, SmtpPort, SecureSocketOptions.Auto);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
