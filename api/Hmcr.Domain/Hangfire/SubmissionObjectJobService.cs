using Hangfire;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
            _user.Username = "hangfire";
            _user.UserGuid = new Guid();

            var jobRunId = Guid.NewGuid().ToString("N");
            var scope = new Dictionary<string, object>
            {
                ["timestampUtc"] = DateTime.UtcNow,
                ["source"] = HmcrLogConstants.Sources.Hangfire,
                ["operation"] = "RunReportingJob",
                ["actorType"] = HmcrLogConstants.ActorTypes.System,
                ["actorUsername"] = _user.Username,
                ["actorDirectory"] = _user.AuthDirName,
                ["correlationId"] = $"hmcr-{jobRunId}",
                ["jobRunId"] = jobRunId,
                ["serviceAreaNumber"] = serviceAreaNumber
            };

            using (_logger.BeginScope(scope))
            {
                var submissions = _submissionRepo.GetSubmissionObjecsForBackgroundJob(serviceAreaNumber);

                if (submissions.Length == 0)
                    return;

                var submissionIds = string.Join(",", submissions.Select(x => (long)x.SubmissionObjectId).ToArray());

                _logger.LogInformation(
                    "[Hangfire] Reporting job for service area {ServiceAreaNumber} is starting to process submissions {SubmissionIds}",
                    serviceAreaNumber,
                    submissionIds);

                foreach (var submission in submissions)
                {
                    using (_logger.BeginScope(new Dictionary<string, object>
                    {
                        ["submissionObjectId"] = (long)submission.SubmissionObjectId,
                        ["submissionStream"] = submission.StagingTableName,
                        ["serviceAreaNumber"] = submission.ServiceAreaNumber
                    }))
                    {
                        switch (submission.StagingTableName)
                        {
                            case TableNames.WorkReport:
                                if (!await _workRptJobService.ProcessSubmissionMain(submission))
                                {
                                    LogConcurrentJobWarning(submission);
                                    return;
                                }
                                break;
                            case TableNames.RockfallReport:
                                if (!await _rockfallRptJobService.ProcessSubmissionMain(submission))
                                {
                                    LogConcurrentJobWarning(submission);
                                    return;
                                }
                                break;
                            case TableNames.WildlifeReport:
                                if (!await _wildlifeRptJobService.ProcessSubmissionMain(submission))
                                {
                                    LogConcurrentJobWarning(submission);
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

        private void LogConcurrentJobWarning(SubmissionDto submission)
        {
            _logger.LogWarning(
                "[Hangfire] Detected another instance of the job. Cancelling service area {ServiceAreaNumber} submission {SubmissionObjectId}.",
                submission.ServiceAreaNumber,
                submission.SubmissionObjectId);
        }
    }
}
