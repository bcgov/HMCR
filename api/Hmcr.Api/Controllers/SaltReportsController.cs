using Microsoft.AspNetCore.Mvc;
using Hmcr.Api.Controllers.Base;
using Hmcr.Model.Dtos.SaltReport;
using Hmcr.Data.Database.Entities;
using System;
using System.Threading.Tasks;
using Hmcr.Domain.Services;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/saltreports")]
    [ApiController]
    public class SaltReportController : HmcrControllerBase
    {
        private readonly ISaltReportService _saltReportService;

        public SaltReportController(ISaltReportService saltReportService)
        {
            _saltReportService = saltReportService ?? throw new ArgumentNullException(nameof(saltReportService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSaltReportAsync([FromBody] SaltReportDto saltReport)
        {
            if (saltReport == null)
            {
                return BadRequest("Received salt report data is null");
            }

            try
            {
                var createdReport = await _saltReportService.CreateReportAsync(saltReport);

                return CreatedAtRoute(nameof(GetSaltReportAsync), new { id = createdReport.SaltReportId }, saltReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occured: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSaltReportsAsync()
        {
            try
            {
                var csvStream = await _saltReportService.ExportReportsToCsvAsync();

                if (csvStream == null || csvStream.Length == 0)
                {
                    return NotFound("CSV data not found or is empty.");
                }

                // Return the stream as a file
                return File(csvStream, "text/csv", "salt_reports.csv");
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return StatusCode(500, "An error occurred while generating the report.");
            }
        }

        [HttpGet("{id}", Name = "GetSaltReportAsync")]
        public async Task<ActionResult<SaltReportDto>> GetSaltReportAsync(int id)
        {
            var saltReportDto = await _saltReportService.GetSaltReportByIdAsync(id);

            if (saltReportDto == null)
            {
                return NotFound();
            }

            return Ok(saltReportDto);
        }
    }
}