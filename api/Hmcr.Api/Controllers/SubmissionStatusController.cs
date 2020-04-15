using Hmcr.Api.Controllers.Base;
using Hmcr.Domain.Services;
using Hmcr.Model.Dtos.SubmissionStream;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/submissionstatus")]
    [ApiController]
    public class SubmissionStatusController : HmcrControllerBase
    {
        private ISubmissionStatusService _statusService;

        public SubmissionStatusController(ISubmissionStatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubmissionStreamDto>>> GetSubmissionStatus()
        {
            return Ok(await _statusService.GetSubmissionStatusAsync());
        }
    }
}
