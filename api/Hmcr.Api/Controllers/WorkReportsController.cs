using Hmcr.Domain.Services;
using Hmcr.Model.Dtos.WorkReport;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost]
        public async Task<IActionResult> CreateWorkReportAsync([FromForm] WorkRptUploadDto upload)
        {
            var result = await _workRptService.PerformInitialValidation(upload);

            if (result.SubmissionObjectId == 0)
            {
                return ValidationUtils.GetValidationErrorResult(result.Errors, ControllerContext);
            }

            return Ok();
        }
    }
}
