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
        Task<IEnumerable<ActivityRuleDto>> GetRoadLengthRulesAsync();
        Task<IEnumerable<ActivityRuleDto>> GetSurfaceTypeRulesAsync();
        Task<IEnumerable<ActivityRuleDto>> GetRoadClassRulesAsync();
    }

    public class ActivityRuleRepository : HmcrRepositoryBase<HmrServiceArea>, IActivityRuleRepository //TODO
    {
        public ActivityRuleRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<ActivityRuleDto>> GetRoadLengthRulesAsync()
        {
            //TODO
            //var activityRules = await DbSet.AsNoTracking()
            //    .Where(s => s.HmrActivityRules.Any(sa => sa.TYPE == "RoadLengthRules"))
            //    .ToListAsync();

            //return Mapper.Map<IEnumerable<ActivityRuleDto>>(activityRules);

            //fake data
            ActivityRuleDto[] activityRules = new ActivityRuleDto[3]
            {
                new ActivityRuleDto{ ActivityRuleId = 1, ActivityRuleName = "Road Length Rule 1" },
                new ActivityRuleDto{ ActivityRuleId = 2, ActivityRuleName = "Road Length Rule 2" },
                new ActivityRuleDto{ ActivityRuleId = 3, ActivityRuleName = "Road Length Rule 3" }
            };
            await Task.Delay(1);
            return activityRules;
        }
        public async Task<IEnumerable<ActivityRuleDto>> GetSurfaceTypeRulesAsync()
        {
            //var activityRules = await DbSet.AsNoTracking()
            //    .Where(s => s.HmrActivityRules.Any(sa => sa.TYPE == "SurfaceTypeRules"))
            //    .ToListAsync();

            //return Mapper.Map<IEnumerable<ActivityRuleDto>>(activityRules);

            //fake data
            ActivityRuleDto[] activityRules = new ActivityRuleDto[3]
            {
                new ActivityRuleDto{ ActivityRuleId = 1, ActivityRuleName = "Surface Type Rule 1" },
                new ActivityRuleDto{ ActivityRuleId = 2, ActivityRuleName = "Surface Type Rule 2" },
                new ActivityRuleDto{ ActivityRuleId = 3, ActivityRuleName = "Surface Type Rule 3" }
            };
            await Task.Delay(1);
            return activityRules;
        }
        public async Task<IEnumerable<ActivityRuleDto>> GetRoadClassRulesAsync()
        {
            //var activityRules = await DbSet.AsNoTracking()
            //    .Where(s => s.HmrActivityRules.Any(sa => sa.TYPE == "RoadClass"))
            //    .ToListAsync();

            //return Mapper.Map<IEnumerable<ActivityRuleDto>>(activityRules);

            //fake data
            ActivityRuleDto[] activityRules = new ActivityRuleDto[3]
            {
                new ActivityRuleDto{ ActivityRuleId = 1, ActivityRuleName = "Road Class Rule 1" },
                new ActivityRuleDto{ ActivityRuleId = 2, ActivityRuleName = "Road Class Rule 2" },
                new ActivityRuleDto{ ActivityRuleId = 3, ActivityRuleName = "Road Class Rule 3" }
            };
            await Task.Delay(1);
            return activityRules;
        }
    }
}
