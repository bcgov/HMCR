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
    [Route("api/wildlifereports")]
    public class WildlifeReportsController : HmcrControllerBase
    {
        private IWildlifeReportService _wildlifeRptService;
        private ISubmissionObjectService _submissionService;
        private HmcrCurrentUser _currentUser;

        public WildlifeReportsController(IWildlifeReportService wildlifeRptService, ISubmissionObjectService submissionService, HmcrCurrentUser currentUser)
        {
            _wildlifeRptService = wildlifeRptService;
            _submissionService = submissionService;
            _currentUser = currentUser;
        }

        [HttpPost]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<IActionResult> CreateWorkReportAsync([FromForm] FileUploadDto upload)
        {
            var problem = IsServiceAreaAuthorized(_currentUser, upload.ServiceAreaNumber);
            if (problem != null)
            {
                return Unauthorized(problem);
            }

            var (SubmissionObjectId, Errors) = await _wildlifeRptService.CreateReportAsync(upload);

            if (Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(Errors, ControllerContext);
            }

            return CreatedAtRoute("GetSubmissionObject", new { id = SubmissionObjectId }, await _submissionService.GetSubmissionObjectAsync(SubmissionObjectId));
        }

        [HttpPost("duplicates")]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public ActionResult<List<string>> CheckResubmit([FromForm] FileUploadDto upload)
        {
            var problem = IsServiceAreaAuthorized(_currentUser, upload.ServiceAreaNumber);
            if (problem != null)
            {
                return Unauthorized(problem);
            }
            
            return Ok(new List<string>());
        }
    }
}
