using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Hmcr.Api
{
    public static class ValidationUtils
    {
        public static UnprocessableEntityObjectResult GetValidationErrorResult(Dictionary<string, List<string>> messages, ActionContext context)
        {
            var errors = new Dictionary<string, string[]>();

            foreach (var message in messages)
            {
                errors.Add(message.Key, message.Value.ToArray());
            }

            var problem = new ValidationProblemDetails(errors)
            {
                Type = "https://hmcr.bc.gov.ca/model-validation-error",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = "Please refer to the errors property for additional details",
                Instance = context.HttpContext.Request.Path
            };

            problem.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

            return new UnprocessableEntityObjectResult(problem)
            {
                ContentTypes = { "application/problem+json" }
            };
        }

        public static UnprocessableEntityObjectResult GetValidationErrorResult(ActionContext context)
        {
            var problem = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://hmcr.bc.gov.ca/model-validation-error",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = "Please refer to the errors property for additional details",
                Instance = context.HttpContext.Request.Path
            };

            problem.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

            return new UnprocessableEntityObjectResult(problem)
            {
                ContentTypes = { "application/problem+json" }
            };
        }

        public static UnprocessableEntityObjectResult GetServiceAreasMissingErrorResult(ActionContext context)
        {
            var problem = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://hmcr.bc.gov.ca/model-validation-error",
                Title = "Service areas missing",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = "Please include service areas in the query string as comma separated string.",
                Instance = context.HttpContext.Request.Path
            };

            problem.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

            return new UnprocessableEntityObjectResult(problem)
            {
                ContentTypes = { "application/problem+json" }
            };
        }

        public static UnprocessableEntityObjectResult GetValidationErrorResult(ActionContext context, string title, string detail)
        {
            var problem = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://hmcr.bc.gov.ca/model-validation-error",
                Title = title,
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = detail,
                Instance = context.HttpContext.Request.Path
            };

            problem.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

            return new UnprocessableEntityObjectResult(problem)
            {
                ContentTypes = { "application/problem+json" }
            };
        }

        public static UnprocessableEntityObjectResult GetValidationErrorResult(ActionContext context, int status, string title, string detail)
        {
            var problem = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://hmcr.bc.gov.ca/model-validation-error",
                Title = title,
                Status = status,
                Detail = detail,
                Instance = context.HttpContext.Request.Path
            };

            problem.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

            return new UnprocessableEntityObjectResult(problem)
            {
                ContentTypes = { "application/problem+json" }
            };
        }

        public static UnprocessableEntityObjectResult GetValidationErrorResult(ValidationProblemDetails problem)
        {
            return new UnprocessableEntityObjectResult(problem)
            {
                ContentTypes = { "application/problem+json" }
            };
        }
    }
}
