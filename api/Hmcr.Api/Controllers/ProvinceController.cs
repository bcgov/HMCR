using Hmcr.Api.Authorization;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/provinces")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        private IProvinceService _provinceService;

        public ProvinceController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        [Route("")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProvinceDto>>> GetProvincesAsync()
        {
            return Ok(await _provinceService.GetProvincesAsync());
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> AddProvince([FromBody]ProvinceDto province)
        {
            await _provinceService.AddProvinceAsync(province);

            return Ok();
        }

        [Route("exception")]
        [HttpGet]
        public IActionResult GetException()
        {
            throw new Exception("Sample Exception");
        }

        [Route("paged")]
        [HttpGet]
        public ActionResult<PagedDto<ProvinceDto>> GetPaged()
        {
            return Ok(_provinceService.GetProvincesByPage(3, 2));
        }

        [Route("authorized")]
        [HttpGet]
        [RequiresPermission(Permissions.Read)]
        public async Task<ActionResult<IEnumerable<ProvinceDto>>> OnlyForAuthorizedUsers()
        {
            return Ok(await _provinceService.GetProvincesAsync());
        }
    }
}