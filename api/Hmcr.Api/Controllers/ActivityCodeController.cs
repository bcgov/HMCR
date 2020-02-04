using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hmcr.Api.Authorization;
using Hmcr.Api.Controllers.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.ActivityCode;
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
        //does this require read permissions?
        public async Task<ActionResult<PagedDto<ActivityCodeSearchDto>>> GetActivityCodesAsync(
            [FromQuery]string? maintenanceTypes, [FromQuery]string? locationCodes, [FromQuery]string? status, [FromQuery]string searchText,
            [FromQuery]int pageSize, [FromQuery]int pageNumber, [FromQuery]string orderBy = "activitynumber", [FromQuery]string direction = "desc")
        {
            ActivityCodeSearchDto[] acd = new ActivityCodeSearchDto[3]
            {
                new ActivityCodeSearchDto{ ActivityCodeId = 1, ActivityNumber = "101200", ActivityName = "Temporary Patching", UnitOfMeasure = "num", MaintenanceType = "Routine", LocationCodeId = 3, PointLineFeature = null, EndDate = null, IsActive = true, CanDelete = true },
                new ActivityCodeSearchDto{ ActivityCodeId = 2, ActivityNumber = "101300", ActivityName = "Overlay Patch", UnitOfMeasure = "tonne", MaintenanceType = "Quantified", LocationCodeId = 1, PointLineFeature = "Either", EndDate = null, IsActive = true , CanDelete = false},
                new ActivityCodeSearchDto{ ActivityCodeId = 3, ActivityNumber = "101301", ActivityName = "Overlay Patch Isolated", UnitOfMeasure = "tonne", MaintenanceType = "Quantified", LocationCodeId = 1, PointLineFeature = "Either", EndDate = DateTime.Today.AddDays(-2), IsActive = false , CanDelete = false},
            };
            
            var pagedDTO = new PagedDto<ActivityCodeSearchDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 1,
                SourceList = acd,
                OrderBy = orderBy,
                Direction = direction
            };

            return pagedDTO;
        }

        [HttpGet("{id}", Name = "GetActivityCode")]
        public async Task<ActionResult<ActivityCodeSearchDto>> GetActivityCodeAsync(decimal id)
        {
            ActivityCodeSearchDto acd = new ActivityCodeSearchDto
            { 
                ActivityCodeId = id, 
                ActivityNumber = "101200", 
                ActivityName = "Temporary Patching", 
                UnitOfMeasure = "num", 
                MaintenanceType = "R", 
                LocationCodeId = 1, 
                PointLineFeature = "L", 
            };

            return acd;
        }

        [HttpPost]
        [RequiresPermission(Permissions.CodeWrite)]
        public async Task<ActionResult<ActivityCodeDto>> CreateActivityCode(ActivityCodeCreateDto activityCode)
        {
            //create activity code async

            ActivityCodeDto acd = new ActivityCodeDto
            {
                ActivityCodeId = 9999,
                ActivityNumber = activityCode.ActivityNumber,
                ActivityName = activityCode.ActivityName,
                UnitOfMeasure = activityCode.UnitOfMeasure,
                MaintenanceType = activityCode.MaintenanceType,
                LocationCodeId = activityCode.LocationCodeId,
                PointLineFeature = activityCode.PointLineFeature,
                EndDate = activityCode.EndDate
            };

            return acd;
        }

        [HttpPut("{id}")]
        [RequiresPermission(Permissions.CodeWrite)]
        public async Task<ActionResult> UpdateActivityCode(decimal id, ActivityCodeUpdateDto activityCode)
        {
            if (id != activityCode.ActivityCodeId)
            {
                throw new Exception($"The Activity Code ID from the query string does not match that of the body.");
            }

            return NoContent();
        }
    }
}
