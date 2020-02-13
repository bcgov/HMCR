using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hmcr.Api.Authorization;
using Hmcr.Api.Controllers.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.LocationCode;
using Hmcr.Model.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/activitycodes")]
    [ApiController]
    public class ActivityCodeController : HmcrControllerBase
    {
        private IActivityCodeService _activityCodeSvc;
        
        public ActivityCodeController(IActivityCodeService activityCodeSvc)
        {
            _activityCodeSvc = activityCodeSvc;
        }

        [HttpGet]
        [RequiresPermission(Permissions.CodeRead)]
        public async Task<ActionResult<PagedDto<ActivityCodeSearchDto>>> GetActivityCodesAsync(
            string? maintenanceTypes, string? locationCodes, bool? isActive, string? searchText,
            int pageSize, int pageNumber, string orderBy = "activitynumber", string direction = "desc")
        {
            return Ok(await _activityCodeSvc.GetActivityCodesAsync(maintenanceTypes.ToStringArray(), locationCodes.ToDecimalArray(), isActive, searchText, pageSize, pageNumber, orderBy, direction));
        }

        [HttpGet("{id}", Name = "GetActivityCode")]
        public async Task<ActionResult<ActivityCodeSearchDto>> GetActivityCodeAsync(decimal id)
        {
            return Ok(await _activityCodeSvc.GetActivityCodeAsync(id));
        }

        [HttpPost]
        [RequiresPermission(Permissions.CodeWrite)]
        public async Task<ActionResult<ActivityCodeDto>> CreateActivityCode(ActivityCodeCreateDto activityCode)
        {
            var response = await _activityCodeSvc.CreateActivityCodeAsync(activityCode);

            if (response.Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(response.Errors, ControllerContext);
            }

            return CreatedAtRoute("GetActivityCode", new { response.id }, await _activityCodeSvc.GetActivityCodeAsync(response.id));
        }

        [HttpPut("{id}")]
        [RequiresPermission(Permissions.CodeWrite)]
        public async Task<ActionResult> UpdateActivityCode(decimal id, ActivityCodeUpdateDto activityCode)
        {
            if (id != activityCode.ActivityCodeId)
            {
                throw new Exception($"The Activity Code ID from the query string does not match that of the body.");
            }

            var response = await _activityCodeSvc.UpdateActivityCodeAsync(activityCode);

            if (response.NotFound)
            {
                return NotFound();
            }

            if (response.Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(response.Errors, ControllerContext);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [RequiresPermission(Permissions.CodeWrite)]
        public async Task<ActionResult> DeleteActivityCode(decimal id, ActivityCodeDeleteDto activityCode)
        {
            (bool NotFound, Dictionary<string, List<string>> Errors) response;

            if (id != activityCode.ActivityCodeId)
            {
                throw new Exception($"The activity code ID from the query string does not match that of the body.");
            }

            if (activityCode.EndDate != null)
            {
                response = await _activityCodeSvc.DisableActivityCodeAsync(activityCode);
            } else
            {
                response = await _activityCodeSvc.DeleteActivityCodeAsync(activityCode);
            }

            if (response.NotFound)
            {
                return NotFound();
            }

            if (response.Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(response.Errors, ControllerContext);
            }

            return NoContent();
        }
    }
}
