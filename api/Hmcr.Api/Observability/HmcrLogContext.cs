using Hmcr.Model;
using Hmcr.Model.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hmcr.Api.Observability
{
    public static class HmcrLogContext
    {
        private const string CorrelationItemKey = "__HmcrCorrelationId";
        private const int MaxIdentifierLength = 100;

        public static string CreateSupportId()
        {
            return HmcrSupportId.Create();
        }

        public static string EnsureCorrelationId(HttpContext context)
        {
            if (context.Items.TryGetValue(CorrelationItemKey, out var existing) && existing is string existingId)
                return existingId;

            var correlationId = GetHeaderValue(context, HmcrLogConstants.CorrelationHeader);
            if (!IsSafeIdentifier(correlationId))
                correlationId = $"hmcr-{Guid.NewGuid():N}";

            context.Items[CorrelationItemKey] = correlationId;
            return correlationId;
        }

        public static void SetResponseHeaders(HttpContext context, string supportId = null)
        {
            if (context.Response.HasStarted)
                return;

            context.Response.Headers[HmcrLogConstants.CorrelationHeader] = EnsureCorrelationId(context);

            if (!string.IsNullOrWhiteSpace(supportId))
                context.Response.Headers[HmcrLogConstants.SupportIdHeader] = supportId;
        }

        public static void EnrichProblemDetails(ProblemDetails problem, HttpContext context, string supportId = null, string errorCode = null)
        {
            problem.Extensions["traceId"] = context.TraceIdentifier;
            problem.Extensions["correlationId"] = EnsureCorrelationId(context);
            problem.Extensions["timestampUtc"] = DateTime.UtcNow.ToString("O");

            if (!string.IsNullOrWhiteSpace(supportId))
                problem.Extensions["supportId"] = supportId;

            if (!string.IsNullOrWhiteSpace(errorCode))
                problem.Extensions["errorCode"] = errorCode;

            SetResponseHeaders(context, supportId);
        }

        public static ValidationProblemDetails CreateUnexpectedProblem(HttpContext context, string supportId, string errorCode)
        {
            var problem = new ValidationProblemDetails()
            {
                Type = "https://hmcr.bc.gov.ca/exception",
                Title = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = $"Contact support with support ID {supportId}.",
                Instance = $"urn:hmcr:error:{supportId}"
            };

            EnrichProblemDetails(problem, context, supportId, errorCode);
            return problem;
        }

        public static Dictionary<string, object> CreateHttpScope(
            HttpContext context,
            HmcrCurrentUser currentUser,
            string source,
            string operation = null,
            string supportId = null,
            string errorCode = null,
            int? statusCode = null,
            long? elapsedMs = null)
        {
            var scope = new Dictionary<string, object>
            {
                ["timestampUtc"] = DateTime.UtcNow,
                ["source"] = source,
                ["operation"] = operation ?? GetOperation(context),
                ["correlationId"] = EnsureCorrelationId(context),
                ["traceId"] = context.TraceIdentifier,
                ["httpMethod"] = context.Request.Method,
                ["requestPath"] = context.Request.Path.Value
            };

            AddActor(scope, currentUser, HmcrLogConstants.ActorTypes.Anonymous);
            AddIfPresent(scope, "supportId", supportId);
            AddIfPresent(scope, "errorCode", errorCode);
            AddIfPresent(scope, "statusCode", statusCode);
            AddIfPresent(scope, "elapsedMs", elapsedMs);

            return scope;
        }

        public static Dictionary<string, object> CreateSystemScope(
            string source,
            string operation,
            HmcrCurrentUser currentUser = null,
            string supportId = null,
            string errorCode = null)
        {
            var scope = new Dictionary<string, object>
            {
                ["timestampUtc"] = DateTime.UtcNow,
                ["source"] = source,
                ["operation"] = operation,
                ["correlationId"] = $"hmcr-{Guid.NewGuid():N}",
                ["actorType"] = HmcrLogConstants.ActorTypes.System
            };

            AddActor(scope, currentUser, HmcrLogConstants.ActorTypes.System);
            AddIfPresent(scope, "supportId", supportId);
            AddIfPresent(scope, "errorCode", errorCode);

            return scope;
        }

        public static string GetOperation(HttpContext context)
        {
            return context.GetEndpoint()?.DisplayName ?? $"{context.Request.Method} {context.Request.Path.Value}";
        }

        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;

            return value.Substring(0, maxLength);
        }

        private static void AddActor(Dictionary<string, object> scope, HmcrCurrentUser currentUser, string fallbackActorType)
        {
            if (currentUser != null && !string.IsNullOrWhiteSpace(currentUser.Username))
            {
                scope["actorType"] = string.Equals(currentUser.Username, "hangfire", StringComparison.OrdinalIgnoreCase)
                    ? HmcrLogConstants.ActorTypes.System
                    : HmcrLogConstants.ActorTypes.User;
                scope["actorUsername"] = currentUser.Username;
                AddIfPresent(scope, "actorDirectory", currentUser.AuthDirName);
                AddIfPresent(scope, "actorUserGuid", currentUser.UserGuid == Guid.Empty ? null : currentUser.UserGuid.ToString());
                AddIfPresent(scope, "apiClientId", currentUser.ApiClientId);
                return;
            }

            scope["actorType"] = fallbackActorType;
        }

        private static void AddIfPresent(Dictionary<string, object> scope, string key, object value)
        {
            if (value != null)
                scope[key] = value;
        }

        private static string GetHeaderValue(HttpContext context, string headerName)
        {
            if (!context.Request.Headers.TryGetValue(headerName, out var values))
                return null;

            return values.FirstOrDefault();
        }

        private static bool IsSafeIdentifier(string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.Length <= MaxIdentifierLength
                && value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '.' || c == ':');
        }
    }
}
