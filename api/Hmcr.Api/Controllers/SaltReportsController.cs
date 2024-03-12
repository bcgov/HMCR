using Microsoft.AspNetCore.Mvc;
using Hmcr.Api.Controllers.Base;
using Hmcr.Model.Dtos.SaltReport;
using System;
using System.Threading.Tasks;
using Hmcr.Domain.Services;
using Hmcr.Api.Authorization;
using Hmcr.Model;
using System.Net;
using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.SubmissionObject;
using System.Linq;
using System.Text.Json;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/saltreports")]
    [ApiController]
    public class SaltReportController : HmcrControllerBase
    {
        private readonly ISaltReportService _saltReportService;
        private ISubmissionObjectService _submissionService;
        private HmcrCurrentUser _currentUser;
        private IEmailService _emailService;

        public SaltReportController(ISaltReportService saltReportService, ISubmissionObjectService submissionService, HmcrCurrentUser currentUser, IEmailService emailService)
        {
            _saltReportService = saltReportService ?? throw new ArgumentNullException(nameof(saltReportService));
            _submissionService = submissionService;
            _currentUser = currentUser;
            _emailService = emailService;
        }

        [HttpPost]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<IActionResult> CreateSaltReportAsync([FromBody] SaltReportDto saltReport)
        {
            var problem = IsServiceAreaAuthorized(_currentUser, saltReport.ServiceArea);

            if (problem != null)
            {
                return Unauthorized(problem);
            }

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

        [HttpGet("{param?}", Name = "GetSaltReportsAsync")]
        public async Task<IActionResult> GetSaltReportsAsync(string serviceAreas, string format, DateTime fromDate, DateTime toDate, string typeName)
        {
            try
            {

                // Decide the output format based on the 'format' parameter
                switch (format.ToLower())
                {
                    case "csv":
                        var saltReportEntities = await _saltReportService.GetSaltReportEntitiesAsync(serviceAreas, fromDate, toDate, typeName);

                        if (!saltReportEntities.Any())
                        {
                            return NotFound("No reports found matching the specified criteria.");
                        }
                        var csvStream = _saltReportService.ConvertToCsvStream(saltReportEntities);
                        if (csvStream == null || csvStream.Length == 0)
                        {
                            return NotFound("CSV data not found or is empty.");
                        }
                        // Return the stream as a CSV file
                        return File(csvStream, "text/csv", "salt_reports.csv");

                    case "json":
                    default:
                        var saltReportDtos = await _saltReportService.GetSaltReportDtosAsync(serviceAreas, fromDate, toDate, typeName);
                        var json = JsonSerializer.Serialize(saltReportDtos);
                        return Content(json, "application/json; charset=utf-8");
                }
            }
            catch (Exception ex)
            {
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