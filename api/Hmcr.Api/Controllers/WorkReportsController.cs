using Hmcr.Api.Authorization;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.WorkReport;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/workreports")]
    public class WorkReportsController : ControllerBase
    {
        private IWorkReportService _workRptService;
        private ISubmissionObjectService _submissionService;

        public WorkReportsController(IWorkReportService workRptService, ISubmissionObjectService submissionService)
        {
            _workRptService = workRptService;
            _submissionService = submissionService;
        }

        [HttpPost]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<IActionResult> CreateWorkReportAsync([FromForm] FileUploadDto upload)
        {
            var (SubmissionObjectId, Errors) = await _workRptService.CreateWorkReportAsync(upload);

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
            var (Errors, DuplicateRecordNumbers) = await _workRptService.CheckDuplicatesAsync(upload);

            if (Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(Errors, ControllerContext);
            }

            return Ok(DuplicateRecordNumbers);
        }
    }
}
