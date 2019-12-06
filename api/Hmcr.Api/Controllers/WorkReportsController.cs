using Hmcr.Api.Authorization;
using Hmcr.Domain.Services;
using Hmcr.Model;
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

        public WorkReportsController(IWorkReportService workRptService)
        {
            _workRptService = workRptService;
        }

        [HttpPost("duplicates")]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<IActionResult> CreateWorkReportAsync([FromForm] WorkRptUploadDto upload)
        {
            var result = await _workRptService.CreateWorkReportAsync(upload);

            if (result.SubmissionObjectId == 0)
            {
                return ValidationUtils.GetValidationErrorResult(result.Errors, ControllerContext);
            }

            return Ok();
        }

        [HttpPost]
        [RequiresPermission(Permissions.FileUploadWrite)]
        public async Task<IActionResult> CheckDuplicateAsync([FromForm] WorkRptUploadDto upload)
        {
            var (Errors, DuplicateRecordNumbers) = await _workRptService.CheckDuplicatesAsync(upload);

            if (Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(Errors, ControllerContext);
            }

            //return Ok(JsonSerializer.Serialize(DuplicateRecordNumbers));
            return Ok(DuplicateRecordNumbers);
        }
    }
}
