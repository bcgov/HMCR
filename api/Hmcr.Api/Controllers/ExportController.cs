using Hmcr.Api.Authorization;
using Hmcr.Api.Controllers.Base;
using Hmcr.Api.Extensions;
using Hmcr.Chris;
using Hmcr.Model;
using Hmcr.Model.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/exports")]
    [ApiController]
    public class ExportController : HmcrControllerBase
    {
        private HmcrCurrentUser _currentUser;
        private IExportApi _exportApi;

        public ExportController(HmcrCurrentUser currentUser, IExportApi exportApi)
        {
            _currentUser = currentUser;
            _exportApi = exportApi;
        }

        [HttpGet("report", Name = "Export")]
        [RequiresPermission(Permissions.FileUploadRead)]
        public async Task<HttpResponse> ExportReport(string? serviceAreas)
        {
            var serviceAreaNumbers = serviceAreas.ToDecimalArray();

            if (serviceAreaNumbers.Length == 0)
            {
                await HttpContext.Response.WriteJsonAsync(ValidationUtils.GetServiceAreasMissingErrorResult(ControllerContext), "application/problem+json");
                return HttpContext.Response;
            }

            var problem = AreServiceAreasAuthorized(_currentUser, serviceAreaNumbers);

            if (problem != null)
            {
                await HttpContext.Response.WriteJsonAsync(problem, "application/problem+json");
                return HttpContext.Response;
            }

            string query = BuildQuery(serviceAreaNumbers);

            var responseMessage = await _exportApi.ExportReport(query);

            await WriteProxiedHttpResponseAsync(responseMessage);

            return HttpContext.Response;
        }

        private string BuildQuery(decimal[] serviceAreaNumbers)
        {
            var saCql = BuildCsqlFromServiceAreaNumbers(serviceAreaNumbers);

            var pq = QueryHelpers.ParseQuery(Request.QueryString.Value);

            if (pq.ContainsKey(Constants.CqlFilter))
            {
                var orgFilter = pq[Constants.CqlFilter].First();
                pq.Remove(Constants.CqlFilter);
                var filter = WebUtility.HtmlEncode($"{saCql} and ({orgFilter})");
                pq.Add(Constants.CqlFilter, filter);
            }
            else
            {
                var filter = WebUtility.HtmlEncode(saCql);
                pq.Add(Constants.CqlFilter, filter);
            }

            pq.Remove(Constants.ServiceAreas);

            var pqkv = new List<KeyValuePair<string, string>>();
            foreach (var kv in pq)
            {
                pqkv.Add(new KeyValuePair<string, string>(kv.Key, kv.Value.First()));
            }

            var bq = new QueryBuilder(pqkv);
            var query = bq.ToQueryString().Value.RemoveQuestionMark();
            return query;
        }

        private string BuildCsqlFromServiceAreaNumbers(decimal[] serviceAreaNumbers)
        {
            var csql = new StringBuilder();

            csql.Append("SERVICE_AREA IN (");

            foreach(var serviceAreaNumber in serviceAreaNumbers)
            {
                csql.Append($"{(int)serviceAreaNumber},");
            }

            return csql.ToString().TrimEnd(',') + ")";
        }

        private Task WriteProxiedHttpResponseAsync(HttpResponseMessage responseMessage)
        {
            var response = HttpContext.Response;

            response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            response.Headers.Remove("transfer-encoding");

            return responseMessage.Content.CopyToAsync(response.Body);
        }
    }
}
