using Hmcr.Api.Authorization;
using Hmcr.Api.Controllers.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionObject;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/submissionobjects")]
    [ApiController]
    public class SubmissionObjectsController : HmcrControllerBase
    {
        private ISubmissionObjectService _submissionService;
        private HmcrCurrentUser _currentUser;

        public SubmissionObjectsController(ISubmissionObjectService submissionService, HmcrCurrentUser currentUser)
        {
            _submissionService = submissionService;
            _currentUser = currentUser;
        }

        [HttpGet("{id}", Name = "GetSubmissionObject")]
        [RequiresPermission(Permissions.FileUploadRead)]
        public async Task<ActionResult<SubmissionObjectDto>> GetSubmissionObjectAsync(decimal id)
        {
            var submission = await _submissionService.GetSubmissionObjectAsync(id);

            if (submission == null)
                return NotFound();

            var problem = IsServiceAreaAuthorized(_currentUser, submission.ServiceAreaNumber);
            if (problem != null)
            {
                return Unauthorized(problem);
            }

            return Ok(submission);
        }

        [HttpGet]
        [RequiresPermission(Permissions.FileUploadRead)]
        public async Task<ActionResult<IEnumerable<SubmissionObjectSearchDto>>> GetSubmissionObjectsAsync(decimal serviceAreaNumber, DateTime dateFrom, DateTime dateTo, 
            int pageSize, int pageNumber, string searchText = null, string orderBy = "AppCreateTimestamp", string direction = "desc")
        {
            var problem = IsServiceAreaAuthorized(_currentUser, serviceAreaNumber);
            if (problem != null)
            {
                return Unauthorized(problem);
            }

            return Ok(await _submissionService.GetSubmissionObjectsAsync(serviceAreaNumber, dateFrom, dateTo, pageSize, pageNumber, searchText, orderBy, direction));
        }

        [HttpGet("{id}/result", Name = "GetSubmissionResult")]
        [RequiresPermission(Permissions.FileUploadRead)]
        public async Task<ActionResult<SubmissionObjectResultDto>> GetSubmissionResultAsync(decimal id)
        {
            var submission = await _submissionService.GetSubmissionResultAsync(id);

            if (submission == null)
                return NotFound();

            var problem = IsServiceAreaAuthorized(_currentUser, submission.ServiceAreaNumber);
            if (problem != null)
            {
                return Unauthorized(problem);
            }

            return Ok(submission);
        }

        [HttpGet("{id}/file", Name = "GetSubmissionFile")]
        [RequiresPermission(Permissions.FileUploadRead)]
        public async Task<IActionResult> GetSubmissionFileAsync(decimal id)
        {
            var submission = await _submissionService.GetSubmissionFileAsync(id);

            if (submission == null)
                return NotFound();

            var problem = IsServiceAreaAuthorized(_currentUser, submission.ServiceAreaNumber);
            if (problem != null)
            {
                return Unauthorized(problem);
            }

            return File(submission.DigitalRepresentation, submission.MimeTypeCode, submission.FileName);
        }

        [HttpGet("{id}/exportcsv", Name = "ExportCsv")]
        [RequiresPermission(Permissions.Export)]
        public async Task<IActionResult> ExportCsvAsync(decimal id)
        {
            var (submission, file) = await _submissionService.ExportSubmissionCsvAsync(id);

            if (submission == null || file == null)
                return NotFound();

            var problem = IsServiceAreaAuthorized(_currentUser, submission.ServiceAreaNumber);
            if (problem != null)
            {
                return Unauthorized(problem);
            }

            return File(file, submission.MimeTypeCode, submission.FileName);
        }
    }
}
