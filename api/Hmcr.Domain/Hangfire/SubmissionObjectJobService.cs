using Hangfire;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.User;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
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

            var submissions = Array.Empty<SubmissionDto>();

            try
            {
                //Jobs must be processed chronologically. GetSubmissionObjecsForBackgroundJobAsync returns submissions by ascending order
                submissions = _submissionRepo.GetSubmissionObjecsForBackgroundJob(serviceAreaNumber); //todo: get staged rows too
            }
            catch (SqlException) //connection timeout happens when a long transaction is running but the issue gets resolved as the transaction finishes. looks like a Hangfire issue
            {
                _logger.LogWarning($"[Hangfire] RunReportingJob for service area {serviceAreaNumber} - connection timeout.");
            }

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
