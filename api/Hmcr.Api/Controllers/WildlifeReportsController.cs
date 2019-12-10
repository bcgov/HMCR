using Hmcr.Api.Authorization;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/wildlifereports")]
    public class WildlifeReportsController : ControllerBase
    {
        private IWildlifeReportService _wildlifeRptService;
        private ISubmissionObjectService _submissionService;

        public WildlifeReportsController(IWildlifeReportService wildlifeRptService, ISubmissionObjectService submissionService)
        {
            _wildlifeRptService = wildlifeRptService;
            _submissionService = submissionService;
        }

        [HttpPost]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<IActionResult> CreateWorkReportAsync([FromForm] FileUploadDto upload)
        {
            var (SubmissionObjectId, Errors) = await _wildlifeRptService.CreateReportAsync(upload);

            if (SubmissionObjectId == 0)
            {
                return ValidationUtils.GetValidationErrorResult(Errors, ControllerContext);
            }

            return CreatedAtRoute("GetSubmissionObject", new { id = SubmissionObjectId }, await _submissionService.GetSubmissionObjectAsync(SubmissionObjectId));
        }
    }
}
