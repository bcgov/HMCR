using Hmcr.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Hmcr.Api.Controllers.Base
{
    public class HmcrControllerBase : ControllerBase
    {
        protected ValidationProblemDetails IsServiceAreaAuthorized(HmcrCurrentUser currentUser, decimal serviceAreaNumber)
        {
            var serviceArea = currentUser.UserInfo.ServiceAreas.FirstOrDefault(x => x.ServiceAreaNumber == serviceAreaNumber);
            if (serviceArea == null)
            {
                var problem = new ValidationProblemDetails()
                {
                    Type = "https://hmcr.bc.gov.ca/exception",
                    Title = "Access denied",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "Insufficient permission.",
                    Instance = HttpContext.Request.Path
                };

                problem.Extensions.Add("traceId", HttpContext.TraceIdentifier);

                return problem;
            }

            return null;
        }
    }
}
