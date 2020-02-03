using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hmcr.Api.Controllers.Base;
using Hmcr.Domain.Services;
using Hmcr.Model.Dtos.LocationCode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/locationcodes")]
    [ApiController]
    public class LocationCodeController : HmcrControllerBase
    {
        private ILocationCodeService _locationCodeService;

        public LocationCodeController(ILocationCodeService locationCodeService) 
        {
            _locationCodeService = locationCodeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationCodeDropDownDto>>> GetLocationCodes()
        {
            return Ok(await _locationCodeService.GetLocationCodesAsync());
        }
    }
}
