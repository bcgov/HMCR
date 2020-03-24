using Hmcr.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        protected ValidationProblemDetails AreServiceAreasAuthorized(HmcrCurrentUser currentUser, decimal[] serviceAreaNumbers)
        {
            var illegalNumbers = new List<decimal>();

            foreach(var serviceAreaNumber in serviceAreaNumbers)
            {
                if (!currentUser.UserInfo.ServiceAreas.Any(x => x.ServiceAreaNumber == serviceAreaNumber))
                {
                    illegalNumbers.Add(serviceAreaNumber);
                }
            }

            if (illegalNumbers.Count == 0)
            {
                return null;
            }

            var message = new StringBuilder("User doesn't have access to the service ares(s) - ");


            foreach(var number in illegalNumbers)
            {
                message.Append($"{number}, ");
            }

            var problem = new ValidationProblemDetails()
            {
                Type = "https://hmcr.bc.gov.ca/model-validation-error",
                Title = "Access denied",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = message.ToString().Trim().TrimEnd(','),
                Instance = HttpContext.Request.Path
            };

            problem.Extensions.Add("traceId", HttpContext.TraceIdentifier);

            return problem;
        }
    }
}
