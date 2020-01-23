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
        Task RunReportingJob(decimal serviceAreaNumber);
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
        private IWildlifeReportJobService _wildlifeRptJobService;

        public SubmissionObjectJobService(ISubmissionObjectRepository submissionRepo, IServiceAreaService svcAreaService, 
            IWorkReportJobService workRptJobService, IRockfallReportJobService rockfallRptJobService, IWildlifeReportJobService wildlifeRptJobService,
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
            _wildlifeRptJobService = wildlifeRptJobService;
        }

        [SkipSameJob]
        [AutomaticRetry(Attempts = 0)]
        public async Task RunReportingJob(decimal serviceAreaNumber)
        {
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
        }
    }
}
