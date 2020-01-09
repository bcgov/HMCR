using Hangfire;
using Hmcr.Data.Repositories;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Hmcr.Domain.Hangfire
{
    public interface ISubmissionObjectJobService
    {
    }

    public class SubmissionObjectJobService : ISubmissionObjectJobService
    {
        private ISubmissionObjectRepository _submissionRepo;
        private IServiceAreaService _svcAreaService;
        private IWorkReportJobService _workRptJobService;
        private IActivityCodeRepository _activityRepo;
        private ISubmissionStatusRepository _statusRepo;
        private HmcrCurrentUser _user;
        private IRockfallReportJobService _rockfallRptJobService;

        public SubmissionObjectJobService(ISubmissionObjectRepository submissionRepo, IServiceAreaService svcAreaService, 
            IWorkReportJobService workRptJobService, IRockfallReportJobService rockfallRptJobService,
            IActivityCodeRepository activityRepo, ISubmissionStatusRepository statusRepo,
            HmcrCurrentUser user)
        {
            _submissionRepo = submissionRepo;
            _svcAreaService = svcAreaService;
            _workRptJobService = workRptJobService;
            _activityRepo = activityRepo;
            _statusRepo = statusRepo;
            _user = user;
            _rockfallRptJobService = rockfallRptJobService;
        }

        public static void RegisterReportingJobs(IEnumerable<ServiceAreaNumberDto> serviceAreas, int minutes)
        {
            if (minutes < 1)
                minutes = 5;

            foreach (var serviceArea in serviceAreas)
            {
                RecurringJob.AddOrUpdate<SubmissionObjectJobService>($"SA{serviceArea.ServiceAreaNumber}", x => x.RunReportingJob(serviceArea.ServiceAreaNumber), $"*/{minutes} * * * *");
            }
        }

        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task RunReportingJob(decimal serviceAreaNumber)
        {
            _user.AuthDirName = UserTypeDto.IDIR;
            _user.UniversalId = "hangfire";
            _user.UserGuid = new Guid();

            //Jobs must be processed chronologically. GetSubmissionObjecsForBackgroundJobAsync returns submissions by ascending order
            var submissions = await _submissionRepo.GetSubmissionObjecsForBackgroundJobAsync(serviceAreaNumber); //todo: get staged rows too

            foreach (var submission in submissions)
            {
                switch (submission.SubmissionStream.StagingTableName)
                {
                    case TableNames.WorkReport:
                        await _workRptJobService.ProcessSubmission(submission);
                        break;
                    case TableNames.RockfallReport:
                        await _rockfallRptJobService.ProcessSubmission(submission);
                        break;
                    case TableNames.WildlifeReport:
                        break;
                    default:
                        throw new NotImplementedException($"Background job for {submission.SubmissionStream.StagingTableName} is not implemented.");
                }
            }
        }
    }
}
