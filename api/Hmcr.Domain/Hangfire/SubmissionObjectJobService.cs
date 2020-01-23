using Hangfire;
using Hmcr.Data.Repositories;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.User;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Hmcr.Domain.Hangfire
{
    public interface ISubmissionObjectJobService
    {
        Task RunReportingJob(decimal serviceAreaNumber);
    }

    public class SubmissionObjectJobService : ISubmissionObjectJobService
    {
        private ISubmissionObjectRepository _submissionRepo;
        private IWorkReportJobService _workRptJobService;
        private IRockfallReportJobService _rockfallRptJobService;
        private IWildlifeReportJobService _wildlifeRptJobService;
        private HmcrCurrentUser _user;
        private ILogger<SubmissionObjectJobService> _logger;

        public SubmissionObjectJobService(ISubmissionObjectRepository submissionRepo, 
            IWorkReportJobService workRptJobService, IRockfallReportJobService rockfallRptJobService, IWildlifeReportJobService wildlifeRptJobService,
            HmcrCurrentUser user, ILogger<SubmissionObjectJobService> logger)
        {
            _submissionRepo = submissionRepo;
            _workRptJobService = workRptJobService;
            _rockfallRptJobService = rockfallRptJobService;
            _wildlifeRptJobService = wildlifeRptJobService;
            _user = user;
            _logger = logger;
        }

        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task RunReportingJob(decimal serviceAreaNumber)
        {
            _logger.LogInformation($"[Hangfire] Starting RunReportingJob for service area {serviceAreaNumber}.");

            _user.AuthDirName = UserTypeDto.IDIR;
            _user.UniversalId = "hangfire";
            _user.UserGuid = new Guid();

            //Jobs must be processed chronologically. GetSubmissionObjecsForBackgroundJobAsync returns submissions by ascending order
            var submissions = _submissionRepo.GetSubmissionObjecsForBackgroundJob(serviceAreaNumber); //todo: get staged rows too

            foreach (var submission in submissions)
            {
                switch (submission.StagingTableName)
                {
                    case TableNames.WorkReport:
                        await _workRptJobService.ProcessSubmission(submission);
                        break;
                    case TableNames.RockfallReport:
                        await _rockfallRptJobService.ProcessSubmission(submission);
                        break;
                    case TableNames.WildlifeReport:
                        await _wildlifeRptJobService.ProcessSubmission(submission);
                        break;
                    default:
                        throw new NotImplementedException($"Background job for {submission.StagingTableName} is not implemented.");
                }
            }

            _logger.LogInformation($"[Hangfire] Finishing RunReportingJob for service area {serviceAreaNumber}.");
        }
    }
}
