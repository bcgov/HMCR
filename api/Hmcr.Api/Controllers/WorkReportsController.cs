using Hangfire;
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
    [Route("api/workreports")]
    public class WorkReportsController : HmcrControllerBase
    {
        private IWorkReportService _workRptService;
        private ISubmissionObjectService _submissionService;
        private HmcrCurrentUser _currentUser;

        public WorkReportsController(IWorkReportService workRptService, ISubmissionObjectService submissionService, HmcrCurrentUser currentUser)
        {
            _workRptService = workRptService;
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

            var (submissionObjectId, errors) = await _workRptService.CreateReportAsync(upload);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            //BackgroundJob.Enqueue<IWorkReportService>(x => x.StartBackgroundProcess(submissionObjectId));

            return CreatedAtRoute("GetSubmissionObject", new { id = submissionObjectId }, await _submissionService.GetSubmissionObjectAsync(submissionObjectId));
        }

        [HttpPost("duplicates")]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<ActionResult<List<string>>> CheckDuplicateAsync([FromForm] FileUploadDto upload)
        {
            var problem = IsServiceAreaAuthorized(_currentUser, upload.ServiceAreaNumber);
            if (problem != null)
            {
                return Unauthorized(problem);
            }

            var (errors, duplicateRecordNumbers) = await _workRptService.CheckDuplicatesAsync(upload);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok(duplicateRecordNumbers);
        }

        [HttpPost("tests/{id}")]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task TestAsync(decimal id)
        {
            await _workRptService.StartBackgroundProcess(id);
        }
    }
}
