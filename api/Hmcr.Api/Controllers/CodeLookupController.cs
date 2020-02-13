using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hmcr.Api.Authorization;
using Hmcr.Api.Controllers.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.CodeLookup;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/codelookup")]
    [ApiController]
    public class CodeLookupController : HmcrControllerBase
    {
        private IFieldValidatorService _validator;

        public CodeLookupController(IFieldValidatorService validator)
        {
            _validator = validator;
        }

        [HttpGet ("maintenancetypes")]
        [RequiresPermission(Permissions.CodeRead)]
        public ActionResult<IEnumerable<CodeLookupForValidation>> GetMaintenanceTypes()
        {
           return Ok(_validator.CodeLookup.Where(x => x.CodeSet == "WRK_RPT_MAINT_TYPE"));
        }

        [HttpGet ("unitofmeasures")]
        [RequiresPermission(Permissions.CodeRead)]
        public ActionResult<IEnumerable<CodeLookupForValidation>> GetUnitOfMeasures()
        {
            return Ok(_validator.CodeLookup.Where(x => x.CodeSet == "UOM"));
        }

        [HttpGet("pointlinefeatures")]
        [RequiresPermission(Permissions.CodeRead)]
        public ActionResult<IEnumerable<CodeLookupForValidation>> GetPointlineFeatures()
        {
            return Ok(_validator.CodeLookup.Where(x => x.CodeSet == "POINT_LINE_FEATURE"));
        }
    }
}