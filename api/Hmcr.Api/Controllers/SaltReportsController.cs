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
using Hmcr.Domain.PdfHelpers;
using System.Reflection;
using Hmcr.Api.Observability;
using Hmcr.Model.Logging;


namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/saltreports")]
    public class SaltReportController : HmcrControllerBase
    {
        private readonly ISaltReportService _saltReportService;
        private readonly ILogger<SaltReportController> _logger;
        private HmcrCurrentUser _currentUser;

        public SaltReportController(ISaltReportService saltReportService, ISubmissionObjectService submissionService, HmcrCurrentUser currentUser, IEmailService emailService, ILogger<SaltReportController> logger)
        {
            _saltReportService = saltReportService ?? throw new ArgumentNullException(nameof(saltReportService));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<IActionResult> CreateSaltReportAsync([FromBody] SaltReportDto saltReport)
        {
            if (saltReport == null)
            {
                return BadRequest("Received salt report data is null");
            }

            var problem = IsServiceAreaAuthorized(_currentUser, saltReport.ServiceArea);
            if (problem != null)
            {
                return Unauthorized(problem);
            }

            try
            {
                var createdReport = await _saltReportService.CreateReportAsync(saltReport);
                return CreatedAtRoute(nameof(GetSaltReportAsync), new { id = createdReport.SaltReportId }, saltReport);
            }
            catch (Exception ex)
            {
                return UnexpectedError(ex, nameof(CreateSaltReportAsync));
            }

        }


        [HttpGet("{id}", Name = "GetSaltReportAsync")]
        public async Task<ActionResult<SaltReportDto>> GetSaltReportAsync(int id, [FromQuery] bool isPdf)
        {
            try
            {
                var saltReportDto = await _saltReportService.GetSaltReportByIdAsync(id);

                if (saltReportDto == null)
                {
                    return NotFound();
                }

                if (isPdf)
                {
                    try
                    {
                        var mapper = new SaltReportPdfMapper();
                        // Map SaltReportDto to a Dictionary<string, string>
                        var pdfData = mapper.MapDtoToPdfData(saltReportDto);

                        // Use the PdfService to fill the PDF
                        var pdfBytes = _saltReportService.FillPdf("saltreport.pdf", pdfData);
                        return File(pdfBytes, "application/pdf", $"salt_report_{id}.pdf");
                    }
                    catch (Exception ex)
                    {
                        return UnexpectedError(ex, nameof(GetSaltReportAsync));
                    }
                }

                return Ok(saltReportDto);
            }
            catch (Exception ex)
            {
                return UnexpectedError(ex, nameof(GetSaltReportAsync));
            }
        }

        [HttpGet(Name = "GetSaltReportsAsync")]
        public async Task<IActionResult> GetSaltReportsAsync([FromQuery] string serviceAreas, [FromQuery] string format, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, int pageSize, int pageNumber, string orderBy = "saltReportId", string direction = "desc")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(serviceAreas))
                {
                    return BadRequest("Service areas cannot be null or empty.");
                }
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
                            var reports = await _saltReportService.GetSaltReportDtosAsync(serviceAreas, fromDate, toDate, pageSize, pageNumber, orderBy, direction);
                            return Ok(reports);
                        }
                        catch (ArgumentException ex)
                        {
                            return BadRequest(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            return UnexpectedError(ex, nameof(GetSaltReportsAsync));
                        }
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request with error message
            }
            catch (DbException ex)
            {
                return UnexpectedError(ex, nameof(GetSaltReportsAsync));
            }
            catch (Exception ex)
            {
                return UnexpectedError(ex, nameof(GetSaltReportsAsync));
            }
        }

        private ObjectResult UnexpectedError(Exception ex, string operation)
        {
            var supportId = HmcrLogContext.CreateSupportId();

            using (_logger.BeginScope(HmcrLogContext.CreateHttpScope(
                HttpContext,
                _currentUser,
                HmcrLogConstants.Sources.Api,
                operation,
                supportId,
                HmcrLogConstants.ErrorCodes.ApiUnexpected,
                500)))
            {
                _logger.LogError(ex, "Unhandled API exception {SupportId}", supportId);
            }

            var problem = HmcrLogContext.CreateUnexpectedProblem(
                HttpContext,
                supportId,
                HmcrLogConstants.ErrorCodes.ApiUnexpected);

            return StatusCode(500, problem);
        }
    }
}
