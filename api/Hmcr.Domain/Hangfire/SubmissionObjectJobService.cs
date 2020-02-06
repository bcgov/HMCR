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
            _user.AuthDirName = UserTypeDto.IDIR;
            _user.UniversalId = "hangfire";
            _user.UserGuid = new Guid();

            var submissions = _submissionRepo.GetSubmissionObjecsForBackgroundJob(serviceAreaNumber);

            foreach (var submission in submissions)
            {
                switch (submission.StagingTableName)
                {
                    case TableNames.WorkReport:
                        if (!await _workRptJobService.ProcessSubmission(submission))
                        {
                            _logger.LogWarning($"[Hangfire] Detected other instance of the job. Cancelling the current job {submission.ServiceAreaNumber} for the submission {submission.SubmissionObjectId}.");
                            return;
                        }
                        break;
                    case TableNames.RockfallReport:
                        if (!await _rockfallRptJobService.ProcessSubmission(submission))
                        {
                            _logger.LogWarning($"[Hangfire] Detected other instance of the job. Cancelling the current job {submission.ServiceAreaNumber} for the submission {submission.SubmissionObjectId}.");
                            return;
                        }
                        break;
                    case TableNames.WildlifeReport:
                        if (!await _wildlifeRptJobService.ProcessSubmission(submission))
                        {
                            _logger.LogWarning($"[Hangfire] Detected other instance of the job. Cancelling the current job {submission.ServiceAreaNumber} for the submission {submission.SubmissionObjectId}.");
                            return;
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Background job for {submission.StagingTableName} is not implemented.");
                }
            }
        }
    }
}
