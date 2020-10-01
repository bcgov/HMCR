using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hmcr.Api.Controllers.Base;
using Hmcr.Model.Dtos.ActivityRule;
using Microsoft.AspNetCore.Mvc;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/activityrule")]
    [ApiController]
    public class ActivityRuleController : HmcrControllerBase
    {
        [HttpGet("roadlength")]
        public ActionResult<IEnumerable<ActivityRuleDto>> GetRoadLengthRules()
        {
            ActivityRuleDto[] activityRules = new ActivityRuleDto[3]
            {
                new ActivityRuleDto{ ActivityRuleId = 1, ActivityRuleName = "Road Length Rule 1" },
                new ActivityRuleDto{ ActivityRuleId = 2, ActivityRuleName = "Road Length Rule 2" },
                new ActivityRuleDto{ ActivityRuleId = 3, ActivityRuleName = "Road Length Rule 3" }
            };

            return activityRules;
        }

        [HttpGet("surfacetype")]
        public ActionResult<IEnumerable<ActivityRuleDto>> GetSurfaceTypeRules()
        {
            ActivityRuleDto[] activityRules = new ActivityRuleDto[3]
            {
                new ActivityRuleDto{ ActivityRuleId = 1, ActivityRuleName = "Surface Type Rule 1" },
                new ActivityRuleDto{ ActivityRuleId = 2, ActivityRuleName = "Surface Type Rule 2" },
                new ActivityRuleDto{ ActivityRuleId = 3, ActivityRuleName = "Surface Type Rule 3" }
            };

            return activityRules;
        }

        [HttpGet("roadclass")]
        public ActionResult<IEnumerable<ActivityRuleDto>> GetRoadClassRules()
        {
            ActivityRuleDto[] activityRules = new ActivityRuleDto[3]
            {
                new ActivityRuleDto{ ActivityRuleId = 1, ActivityRuleName = "Road Class Rule 1" },
                new ActivityRuleDto{ ActivityRuleId = 3, ActivityRuleName = "Road Class Rule 2" },
                new ActivityRuleDto{ ActivityRuleId = 2, ActivityRuleName = "Road Class Rule 3" }
            };

            return activityRules;
        }
    }
}
