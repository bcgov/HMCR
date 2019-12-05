using Hmcr.Api.Authorization;
using Hmcr.Domain.Services;
using Hmcr.Model.Dtos.SubmissionStream;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/submissionstreams")]
    [ApiController]
    public class SubmissionStreamsController : ControllerBase
    {
        private ISubmissionStreamService _streamService;

        public SubmissionStreamsController(ISubmissionStreamService streamService)
        {
            _streamService = streamService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubmissionStreamDto>>> GetSubmissionStreams([FromQuery]bool? isActive)
        {
            return Ok(await _streamService.GetSubmissionStreamsAsync(isActive));
        }
    }
}
