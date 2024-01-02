using Microsoft.AspNetCore.Mvc;
using Hmcr.Api.Controllers.Base;
using Hmcr.Model.Dtos.SaltReport;
using Hmcr.Data.Database.Entities;
using AutoMapper;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Hmcr.Domain.Services;
using System.Linq;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("0")]
    [Route("api/saltreports")]
    [ApiController]
    public class SaltReportController : HmcrControllerBase
    {
        private readonly ISaltReportService _saltReportService;

        public SaltReportController(ISaltReportService saltReportService)
        {
            _saltReportService = saltReportService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSaltReport([FromBody] SaltReportDto saltReportDto)
        {
            if (saltReportDto == null)
            {
                return BadRequest("Received salt report data is null");
            }

            try
            {
                var saltReport = await _saltReportService.CreateSaltReportAsync(saltReportDto);

                return Ok(saltReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occured: " + ex.Message);
            }
        }
    }
}