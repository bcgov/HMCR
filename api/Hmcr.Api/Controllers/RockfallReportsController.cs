using Hmcr.Api.Authorization;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/rockfallreports")]
    public class RockfallReportsController : ControllerBase
    {
        private IRockfallReportService _rockfallRptSerivce;
        private ISubmissionObjectService _submissionService;

        public RockfallReportsController(IRockfallReportService rockfallRptSerivce, ISubmissionObjectService submissionService)
        {
            _rockfallRptSerivce = rockfallRptSerivce;
            _submissionService = submissionService;
        }

        [HttpPost]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<IActionResult> CreateRockfallReportAsync([FromForm] FileUploadDto upload)
        {
            var (SubmissionObjectId, Errors) = await _rockfallRptSerivce.CreateRockfallReportAsync(upload);

            if (SubmissionObjectId == 0)
            {
                return ValidationUtils.GetValidationErrorResult(Errors, ControllerContext);
            }

            return CreatedAtRoute("GetSubmissionObject", new { id = SubmissionObjectId }, await _submissionService.GetSubmissionObjectAsync(SubmissionObjectId));
        }

        [HttpPost("duplicates")]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<ActionResult<List<string>>> CheckDuplicateAsync([FromForm] FileUploadDto upload)
        {
            var (Errors, DuplicateRecordNumbers) = await _rockfallRptSerivce.CheckDuplicatesAsync(upload);

            if (Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(Errors, ControllerContext);
            }

            return Ok(DuplicateRecordNumbers);
        }
    }
}
