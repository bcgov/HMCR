using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.ActivityRule;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IActivityRuleService
    {
        Task<IEnumerable<ActivityRuleDto>> GetRoadLengthRulesAsync();
        Task<IEnumerable<ActivityRuleDto>> GetSurfaceTypeRulesAsync();
        Task<IEnumerable<ActivityRuleDto>> GetRoadClassRulesAsync();
    }

    public class ActivityRuleService : IActivityRuleService
    {
        private IActivityRuleRepository _activityRuleRepo;
        private IUnitOfWork _unitOfWork;

        public ActivityRuleService(IActivityRuleRepository activityRuleRepo, IUnitOfWork unitOfWork)
        {
            _activityRuleRepo = activityRuleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ActivityRuleDto>> GetRoadLengthRulesAsync()
        {
            return await _activityRuleRepo.GetRoadLengthRulesAsync();
        }

        public async Task<IEnumerable<ActivityRuleDto>> GetSurfaceTypeRulesAsync()
        {
            return await  _activityRuleRepo.GetSurfaceTypeRulesAsync();
        }

        public async Task<IEnumerable<ActivityRuleDto>> GetRoadClassRulesAsync()
        {
            return await _activityRuleRepo.GetRoadClassRulesAsync();
        }

    }
}
