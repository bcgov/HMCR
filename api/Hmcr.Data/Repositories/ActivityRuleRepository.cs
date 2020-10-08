using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.ActivityRule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IActivityRuleRepository
    {
        Task<IEnumerable<ActivityCodeRuleDto>> GetRoadLengthRulesAsync();
        Task<IEnumerable<ActivityCodeRuleDto>> GetSurfaceTypeRulesAsync();
        Task<IEnumerable<ActivityCodeRuleDto>> GetRoadClassRulesAsync();
        Task<IEnumerable<ActivityCodeRuleDto>> GetDefaultRules();
    }

    public class ActivityRuleRepository : HmcrRepositoryBase<HmrActivityCodeRule>, IActivityRuleRepository
    {
        public ActivityRuleRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<ActivityCodeRuleDto>> GetRoadLengthRulesAsync()
        {
            var activityRules = await DbSet.AsNoTracking()
                .Where(s => s.ActivityRuleSet.ToUpper() == "ROAD_LENGTH")
                .ToListAsync();

            return Mapper.Map<IEnumerable<ActivityCodeRuleDto>>(activityRules);
        }
        public async Task<IEnumerable<ActivityCodeRuleDto>> GetSurfaceTypeRulesAsync()
        {
            var activityRules = await DbSet.AsNoTracking()
                .Where(s => s.ActivityRuleSet.ToUpper() == "SURFACE_TYPE")
                .ToListAsync();

            return Mapper.Map<IEnumerable<ActivityCodeRuleDto>>(activityRules);
        }
        public async Task<IEnumerable<ActivityCodeRuleDto>> GetRoadClassRulesAsync()
        {
            var activityRules = await DbSet.AsNoTracking()
                .Where(s => s.ActivityRuleSet.ToUpper() == "ROAD_CLASS")
                .ToListAsync();

            return Mapper.Map<IEnumerable<ActivityCodeRuleDto>>(activityRules);
        }

        public async Task<IEnumerable<ActivityCodeRuleDto>> GetDefaultRules()
        {
            var activityRules = await DbSet.AsNoTracking()
                .Where(s => s.ActivityRuleName.ToUpper() == "NOT APPLICABLE")
                .ToListAsync();

            return Mapper.Map<IEnumerable<ActivityCodeRuleDto>>(activityRules);
        }
    }
}
