using Hmcr.Api.Observability;
using Hmcr.Model;
using Hmcr.Model.Dtos.Logging;
using Hmcr.Model.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/clientlogs")]
    [ApiController]
    public class ClientLogsController : ControllerBase
    {
        private readonly ILogger<ClientLogsController> _logger;
        private readonly HmcrCurrentUser _currentUser;

        public ClientLogsController(ILogger<ClientLogsController> logger, HmcrCurrentUser currentUser)
        {
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpPost]
        public ActionResult CreateClientLog([FromBody] ClientLogDto clientLog)
        {
            if (clientLog == null)
                return BadRequest();

            var supportId = IsSafeSupportId(clientLog.SupportId) ? clientLog.SupportId : HmcrLogContext.CreateSupportId();
            var requestedErrorCode = HmcrLogContext.Truncate(clientLog.ErrorCode, 100);
            var errorCode = string.IsNullOrWhiteSpace(requestedErrorCode) ? HmcrLogConstants.ErrorCodes.ClientUnexpected : requestedErrorCode;
            var correlationId = HmcrLogContext.EnsureCorrelationId(HttpContext);
            var timestampUtc = DateTime.UtcNow;
            var requestPath = GetPathWithoutQuery(clientLog.Url);

            var scope = HmcrLogContext.CreateHttpScope(
                HttpContext,
                _currentUser,
                HmcrLogConstants.Sources.Client,
                "ClientLog",
                supportId,
                errorCode,
                clientLog.StatusCode);

            var clientCorrelationId = HmcrLogContext.Truncate(clientLog.CorrelationId, 100);
            scope["correlationId"] = string.IsNullOrWhiteSpace(clientCorrelationId) ? correlationId : clientCorrelationId;
            AddIfPresent(scope, "clientSessionId", HmcrLogContext.Truncate(clientLog.ClientSessionId, 100));
            AddIfPresent(scope, "clientTraceId", HmcrLogContext.Truncate(clientLog.ClientTraceId, 100));
            AddIfPresent(scope, "route", HmcrLogContext.Truncate(clientLog.Route, 300));
            AddIfPresent(scope, "requestPath", requestPath);
            AddIfPresent(scope, "httpMethod", HmcrLogContext.Truncate(clientLog.HttpMethod, 12));
            AddIfPresent(scope, "userAgent", HmcrLogContext.Truncate(clientLog.UserAgent, 500));
            AddIfPresent(scope, "appVersion", HmcrLogContext.Truncate(clientLog.AppVersion, 100));
            AddIfPresent(scope, "clientTimestampUtc", clientLog.TimestampUtc);

            using (_logger.BeginScope(scope))
            {
                _logger.LogError(
                    "Client error reported {SupportId}: {ClientMessage} {ClientStack} {ClientComponentStack}",
                    supportId,
                    HmcrLogContext.Truncate(clientLog.Message, 1000),
                    HmcrLogContext.Truncate(clientLog.Stack, 4000),
                    HmcrLogContext.Truncate(clientLog.ComponentStack, 4000));
            }

            HmcrLogContext.SetResponseHeaders(HttpContext, supportId);

            return Ok(new
            {
                supportId,
                errorCode,
                correlationId,
                timestampUtc = timestampUtc.ToString("O")
            });
        }

        private static bool IsSafeSupportId(string supportId)
        {
            return !string.IsNullOrWhiteSpace(supportId) && supportId.Length <= 100;
        }

        private static void AddIfPresent(System.Collections.Generic.Dictionary<string, object> scope, string key, object value)
        {
            if (value != null)
                scope[key] = value;
        }

        private static string GetPathWithoutQuery(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            if (Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri))
                return HmcrLogContext.Truncate(absoluteUri.AbsolutePath, 300);

            var queryIndex = url.IndexOf("?", StringComparison.Ordinal);
            var path = queryIndex >= 0 ? url.Substring(0, queryIndex) : url;
            return HmcrLogContext.Truncate(path, 300);
        }
    }
}
