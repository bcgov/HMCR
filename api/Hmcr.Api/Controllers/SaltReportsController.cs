using Microsoft.AspNetCore.Mvc;
using Hmcr.Api.Controllers.Base;
using Hmcr.Model.Dtos.SaltReport;
using System;
using System.Threading.Tasks;
using Hmcr.Domain.Services;
using Hmcr.Api.Authorization;
using Hmcr.Model;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Collections.Generic;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/saltreports")]
    public class SaltReportController : HmcrControllerBase
    {
        private readonly ISaltReportService _saltReportService;
        private HmcrCurrentUser _currentUser;
        private readonly ILogger<SaltReportController> _logger;

        public SaltReportController(ISaltReportService saltReportService, ILogger<SaltReportController> logger, ISubmissionObjectService submissionService, HmcrCurrentUser currentUser, IEmailService emailService)
        {
            _saltReportService = saltReportService ?? throw new ArgumentNullException(nameof(saltReportService));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _logger = logger;
        }

        [HttpPost]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<IActionResult> CreateSaltReportAsync([FromBody] SaltReportDto saltReport)
        {
            _logger.LogInformation("Starting to create salt report");
            if (saltReport == null)
            {
                _logger.LogWarning("Received salt report data is null");
                return BadRequest("Received salt report data is null");
            }

            var problem = IsServiceAreaAuthorized(_currentUser, saltReport.ServiceArea);
            if (problem != null)
            {
                _logger.LogWarning($"Service area not authorized: {problem}");
                return Unauthorized(problem);
            }

            try
            {
                var createdReport = await _saltReportService.CreateReportAsync(saltReport);
                _logger.LogInformation($"Salt report created with ID: {createdReport.SaltReportId}");
                return CreatedAtRoute(nameof(GetSaltReportAsync), new { id = createdReport.SaltReportId }, saltReport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in creating salt report");
                return StatusCode(500, "An internal error occurred. Please try again later.");
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

        [HttpGet(Name = "GetSaltReportsAsync")]
        public async Task<IActionResult> GetSaltReportsAsync([FromQuery] string serviceAreas, [FromQuery] string format, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, int pageSize, int pageNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(serviceAreas))
                {
                    return BadRequest("Service areas cannot be null or empty.");
                }
                _logger.LogInformation($"Obtaining Reports for Service area: {serviceAreas}");
                format = (format ?? "json").ToLower();

                // Decide the output format based on the 'format' parameter
                switch (format)
                {
                    case "csv":
                        var saltReportEntities = await _saltReportService.GetSaltReportEntitiesAsync(serviceAreas, fromDate, toDate).ConfigureAwait(false);

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
                        try
                        {
                            var reports = await _saltReportService.GetSaltReportDtosAsync(serviceAreas, fromDate, toDate, pageSize, pageNumber);
                            return Ok(reports);
                        }
                        catch (ArgumentException ex)
                        {
                            return BadRequest(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            // Log the exception details here to diagnose issues.
                            return StatusCode(500, "An internal server error occurred.");
                        }
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument.");
                return BadRequest(ex.Message); // Return 400 Bad Request with error message
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Database exception occurred.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}