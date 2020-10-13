using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hmcr.Api.Controllers.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.ActivityRule;
using Hmcr.Model.Dtos.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/activityrule")]
    [ApiController]
    public class ActivityRuleController : HmcrControllerBase
    {
        private IActivityRuleService _activityRuleSvc;
        public ActivityRuleController(IActivityRuleService activityRuleSvc)
        {
            _activityRuleSvc = activityRuleSvc;
        }

        [HttpGet("roadlength")]
        public async Task<ActionResult<IEnumerable<ActivityCodeRuleDto>>> GetRoadLengthRulesAsync()
        {
            return Ok(await _activityRuleSvc.GetRoadLengthRulesAsync());
        }

        [HttpGet("surfacetype")]
        public async Task<ActionResult<IEnumerable<ActivityCodeRuleDto>>> GetSurfaceTypeRulesAsync()
        {
            return Ok(await _activityRuleSvc.GetSurfaceTypeRulesAsync());
        }

        [HttpGet("roadclass")]
        public async Task<ActionResult<IEnumerable<ActivityCodeRuleDto>>> GetRoadClassRulesAsync()
        {
            return Ok(await _activityRuleSvc.GetRoadClassRulesAsync());
        }
    }
}
