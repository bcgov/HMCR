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

        public SaltReportController(ISaltReportService saltReportService, ISubmissionObjectService submissionService, HmcrCurrentUser currentUser)
        {
            _saltReportService = saltReportService ?? throw new ArgumentNullException(nameof(saltReportService));
            _submissionService = submissionService;
            _currentUser = currentUser;
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

        [HttpGet("{param?}")]
        public async Task<IActionResult> GetSaltReportsAsync(string serviceAreas, DateTime fromDate, DateTime toDate, string cql_filter)
        {
            try
            {
                var saltReportEntities = await _saltReportService.GetSaltReportEntitiesAsync(serviceAreas, fromDate, toDate, cql_filter);

                if (!saltReportEntities.Any())
                {
                    return NotFound("No reports found matching the specified criteria.");
                }
                
                var csvStream = _saltReportService.ConvertToCsvStream(saltReportEntities);

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