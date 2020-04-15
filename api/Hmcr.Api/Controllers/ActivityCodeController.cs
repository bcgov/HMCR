using Hmcr.Api.Authorization;
using Hmcr.Api.Controllers.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.ActivityCode;
using Hmcr.Model.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        [RequiresPermission(Permissions.CodeRead)]
        public async Task<ActionResult<ActivityCodeSearchDto>> GetActivityCodeAsync(decimal id)
        {
            return Ok(await _activityCodeSvc.GetActivityCodeAsync(id));
        }

        [HttpGet("lite", Name = "GetActivityCodesLiteAsync")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ActivityCodeLiteDto>>> GetActivityCodesLiteAsync()
        {
            return Ok(await _activityCodeSvc.GetActivityCodesLiteAsync());
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
        public async Task<ActionResult> DeleteActivityCode(decimal id)
        {
            var response = await _activityCodeSvc.DeleteActivityCodeAsync(id);

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
