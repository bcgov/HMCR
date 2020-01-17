using Hmcr.Api.Authorization;
using Hmcr.Api.Controllers.Base;
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
    public class RockfallReportsController : HmcrControllerBase
    {
        private IRockfallReportService _rockfallRptSerivce;
        private ISubmissionObjectService _submissionService;
        private HmcrCurrentUser _currentUser;

        public RockfallReportsController(IRockfallReportService rockfallRptSerivce, ISubmissionObjectService submissionService, HmcrCurrentUser currentUser)
        {
            _rockfallRptSerivce = rockfallRptSerivce;
            _submissionService = submissionService;
            _currentUser = currentUser;
        }

        [HttpPost]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<IActionResult> CreateRockfallReportAsync([FromForm] FileUploadDto upload)
        {
            var problem = IsServiceAreaAuthorized(_currentUser, upload.ServiceAreaNumber);
            if (problem != null)
            {
                return Unauthorized(problem);
            }

            var (SubmissionObjectId, Errors) = await _rockfallRptSerivce.CreateReportAsync(upload);

            if (Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(Errors, ControllerContext);
            }

            return CreatedAtRoute("GetSubmissionObject", new { id = SubmissionObjectId }, await _submissionService.GetSubmissionObjectAsync(SubmissionObjectId));
        }

        [HttpPost("duplicates")]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<ActionResult<List<string>>> CheckResubmitAsync([FromForm] FileUploadDto upload)
        {
            var problem = IsServiceAreaAuthorized(_currentUser, upload.ServiceAreaNumber);
            if (problem != null)
            {
                return Unauthorized(problem);
            }

            var (Errors, DuplicateRecordNumbers) = await _rockfallRptSerivce.CheckResubmitAsync(upload);

            if (Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(Errors, ControllerContext);
            }

            return Ok(DuplicateRecordNumbers);
        }
    }
}
