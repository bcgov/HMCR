using Hmcr.Api.Authorization;
using Hmcr.Api.Controllers.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionConfiguration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/submissionconfigurations")]
    [ApiController]
    public class SubmissionConfigurationsController : HmcrControllerBase
    {
        private readonly ISubmissionConfigurationService _service;

        public SubmissionConfigurationsController(ISubmissionConfigurationService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequiresPermission(Permissions.SubmissionConfigurationWrite)]
        public async Task<ActionResult<IReadOnlyList<SubmissionConfigurationDto>>> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpPut("{id}/activation")]
        [RequiresPermission(Permissions.SubmissionConfigurationWrite)]
        public async Task<ActionResult<SubmissionConfigurationDto>> UpdateActivationAsync(
            decimal id,
            [FromBody] SubmissionConfigurationActivationDto update)
        {
            var result = await _service.UpdateActivationAsync(
                id,
                update.IsActive.Value,
                update.ConcurrencyControlNumber.Value);

            if (result.Status == SubmissionConfigurationActivationStatus.NotFound)
            {
                return NotFound();
            }

            if (result.Status == SubmissionConfigurationActivationStatus.Conflict)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Submission configuration changed",
                    Detail = "The submission configuration was updated by another user. Refresh and try again.",
                    Status = StatusCodes.Status409Conflict,
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(result.Configuration);
        }

        [HttpGet("notices")]
        [RequiresPermission(Permissions.FileUploadRead)]
        public async Task<ActionResult<IReadOnlyList<SubmissionConfigurationNoticeDto>>> GetNoticesAsync(
            [FromQuery] decimal submissionStreamId,
            [FromQuery] decimal? serviceAreaNumber = null)
        {
            return Ok(await _service.GetNoticesAsync(submissionStreamId, serviceAreaNumber));
        }
    }
}
