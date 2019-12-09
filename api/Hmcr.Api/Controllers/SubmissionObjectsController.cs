using Hmcr.Api.Authorization;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/submissionobjects")]
    [ApiController]
    public class SubmissionObjectsController : ControllerBase
    {
        private ISubmissionObjectService _submissionService;

        public SubmissionObjectsController(ISubmissionObjectService submissionService)
        {
            _submissionService = submissionService;
        }

        [HttpGet("{id}", Name = "GetSubmissionObject")]
        [RequiresPermission(Permissions.FileUploadRead)]
        public async Task<ActionResult<SubmissionObjectDto>> GetSubmissionObjectAsync(decimal id)
        {
            var submission = await _submissionService.GetSubmissionObjectAsync(id);

            if (submission == null)
                return NotFound();

            return submission;
        }
    }
}
