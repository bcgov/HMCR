using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/serviceareas")]
    [ApiController]
    public class ServiceAreasController : ControllerBase
    {
        private IServiceAreaService _svcAreaSvc;

        public ServiceAreasController(IServiceAreaService svcAreaSvc)
        {
            _svcAreaSvc = svcAreaSvc;
        }

        [HttpGet("")]
        public async Task<ActionResult<Task<IEnumerable<ServiceAreaNumberDto>>>> GetAllServiceArea()
        {
            return Ok(await _svcAreaSvc.GetAllServiceAreasAsync());
        }
    }
}
