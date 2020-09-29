using Hangfire;
using Hmcr.Data.Repositories;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.User;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Hangfire
{
    public interface IEmailJobService
    {
        Task ResendEmails();
    }

    public class EmailJobService : IEmailJobService
    {
        private IFeebackMessageRepository _feedbackRepo;
        private IEmailService _emailService;
        private HmcrCurrentUser _user;
        private ILogger<EmailJobService> _logger;

        public EmailJobService(IFeebackMessageRepository feedbackRepo, IEmailService emailService, HmcrCurrentUser user, ILogger<EmailJobService> logger)
        {
            _feedbackRepo = feedbackRepo;
            _emailService = emailService;
            _user = user;
            _logger = logger;
        }

        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task ResendEmails()
        {
            _user.AuthDirName = UserTypeDto.IDIR;
            _user.Username = "hangfire";
            _user.UserGuid = new Guid();

            var feedbackMessages = await _feedbackRepo.GetFailedFeedbackMessagesAsync();
            var count = feedbackMessages.Count();

            if (count == 0)
                return;

            _logger.LogInformation($"[Hangfire] The job for resending emails is starting - {count} emails to send");

            foreach (var feedbackMessage in feedbackMessages)
            {
                if (!await _emailService.SendStatusEmailAsync(feedbackMessage.SubmissionObjectId, feedbackMessage))
                    return;
            }
        }
    }
}
