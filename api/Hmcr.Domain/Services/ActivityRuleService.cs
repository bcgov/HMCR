using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.ActivityRule;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IActivityRuleService
    {
        Task<IEnumerable<ActivityCodeRuleDto>> GetRoadLengthRulesAsync();
        Task<IEnumerable<ActivityCodeRuleDto>> GetSurfaceTypeRulesAsync();
        Task<IEnumerable<ActivityCodeRuleDto>> GetRoadClassRulesAsync();
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

        public async Task<IEnumerable<ActivityCodeRuleDto>> GetRoadLengthRulesAsync()
        {
            return await _activityRuleRepo.GetRoadLengthRulesAsync();
        }

        public async Task<IEnumerable<ActivityCodeRuleDto>> GetSurfaceTypeRulesAsync()
        {
            return await  _activityRuleRepo.GetSurfaceTypeRulesAsync();
        }

        public async Task<IEnumerable<ActivityCodeRuleDto>> GetRoadClassRulesAsync()
        {
            return await _activityRuleRepo.GetRoadClassRulesAsync();
        }

    }
}
