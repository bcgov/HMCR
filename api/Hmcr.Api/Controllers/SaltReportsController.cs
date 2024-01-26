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
using System.IO;

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

        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            try
            {
                var csvStream = await _saltReportService.ExportReportsToCsvAsync();
                if (csvStream == null || csvStream.Length == 0)
                {
                    return NotFound("CSV data not found or is empty.");
                }

                // Return the stream as a file
                return File(csvStream, "text/csv", "report.csv");
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return StatusCode(500, "An error occurred while generating the report.");
            }
        }
    }
}