using Hmcr.Api.Authorization;
using Hmcr.Api.Controllers.Base;
using Hmcr.Chris;
using Hmcr.Model;
using Hmcr.Model.Utils;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System;
using Hmcr.Chris.Models;
using System.Text.Json;


namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/reports")]
    [ApiController]
    public class JsonController : HmcrControllerBase
    {
        private HmcrCurrentUser _currentUser;
        private IJsonApi _exportApi;
        private const String format = "json";

        public JsonController(HmcrCurrentUser currentUser, IJsonApi exportApi)
        {
            _currentUser = currentUser;
            _exportApi = exportApi;
        }



        /// <summary>
        /// Search report records and export in the specified output format
        /// </summary>
        /// <param name="serviceAreas">1 ~ 28</param>
        /// <param name="typeName">hmr:HMR_WORK_REPORT_VW, hmr:HMR_WILDLIFE_REPORT_VW, hmr:HMR_ROCKFALL_REPORT_VW</param>
        /// <param name="fromDate">From date in yyyy-MM-dd format</param>
        /// <param name="toDate">To date in yyyy-MM-dd format</param>
        /// <param name="cql_filter">Filter</param>
        /// <param name="propertyName">Property names of the columns to export</param>
        /// <returns></returns>

        [HttpGet("json", Name = "Report")]
        [RequiresPermission(Permissions.Export)]
        public async Task<IActionResult> ExportJsonFile(string serviceAreas, string typeName, DateTime fromDate, DateTime toDate, string cql_filter, string propertyName)
        {
            var serviceAreaNumbers = serviceAreas.ToDecimalArray();

            if (serviceAreaNumbers.Length == 0)
            {
                serviceAreaNumbers = _currentUser.UserInfo.ServiceAreas.Select(x => x.ServiceAreaNumber).ToArray();
            }

            var invalidResult = ValidateQueryParameters(serviceAreaNumbers, typeName, format, fromDate, toDate);

            if (invalidResult != null)
            {
                return invalidResult;
            }

            var outputFormat = "application/json";
            var endpointConfigName = ExportQueryEndpointConfigName.WFS;

            var dateColName = GetDateColName(typeName);

            var (result, exists) = await MatchExists(serviceAreaNumbers, fromDate, toDate, "text/csv;charset=UTF-8", dateColName, ExportQueryEndpointConfigName.WFS);

            if (result != null)
            {
                return result;
            }

            if (!exists)
            {
                return NotFound();
            }

            var query = BuildQuery(serviceAreaNumbers, fromDate, toDate, outputFormat, dateColName, endpointConfigName, false);

            var responseMessage = await _exportApi.ExportJsonReport(query, endpointConfigName);
            var bytes = await responseMessage.Content.ReadAsByteArrayAsync();
            var content = await responseMessage.Content.ReadAsStringAsync();


            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                return File(bytes, responseMessage.Content.Headers.ContentType.ToString());
            }
            else
            {
                return ValidationUtils.GetValidationErrorResult(ControllerContext,
                     (int)responseMessage.StatusCode, "Error from Geoserver", Encoding.UTF8.GetString(bytes));
            }
        }

        private async Task<(UnprocessableEntityObjectResult result, bool exists)> MatchExists(decimal[] serviceAreaNumbers, DateTime fromDate, DateTime toDate, string outputFormat, string dateColName, string endpointConfigName)
        {
            var query = BuildQuery(serviceAreaNumbers, fromDate, toDate, outputFormat, dateColName, ExportQueryEndpointConfigName.WFS, true);
            var responseMessage = await _exportApi.ExportJsonReport(query, endpointConfigName);

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                var bytes = await responseMessage.Content.ReadAsByteArrayAsync();

                return (ValidationUtils.GetValidationErrorResult(ControllerContext,
                     (int)responseMessage.StatusCode, "Error from Geoserver", Encoding.UTF8.GetString(bytes)), false);
            }

            var content = await responseMessage.Content.ReadAsStringAsync();
            var features = JsonSerializer.Deserialize<FeatureCollection>(content);

            return (null, features.numberMatched > 0);
        }

        private UnprocessableEntityObjectResult ValidateQueryParameters(decimal[] serviceAreaNumbers, string typeName, string outputFormat, DateTime fromDate, DateTime toDate)
        {
            var problem = AreServiceAreasAuthorized(_currentUser, serviceAreaNumbers);

            if (problem != null)
            {
                return ValidationUtils.GetValidationErrorResult(problem);
            }

            if (outputFormat == null)
            {
                return ValidationUtils.GetValidationErrorResult(ControllerContext,
                    "Output format is missing", "Please include a valid output format in the query string.");
            }

            if (typeName == null)
            {
                return ValidationUtils.GetValidationErrorResult(ControllerContext,
                    "Type name is missing", "Please include a valid type name in the query string.");
            }

            var dateFieldName = GetDateColName(typeName);
            if (dateFieldName == null)
            {
                return ValidationUtils.GetValidationErrorResult(ControllerContext,
                    "Type name is invalid", "Please include a valid type name in the query string.");
            }

            if (fromDate < Constants.MinDate)
            {
                return ValidationUtils.GetValidationErrorResult(ControllerContext,
                    "Invalid fromDate", "Please include a valid fromDate in the query string.");
            }

            if (fromDate > toDate)
            {
                return ValidationUtils.GetValidationErrorResult(ControllerContext,
                    "Invalid toDate", "toDate must be greater than fromDate");
            }

            return null;
        }


        private string BuildQuery(decimal[] serviceAreaNumbers, DateTime fromDate, DateTime toDate, string outputFormat, string dateColName, string endpointConfigName, bool count)
        {
            var saCql = BuildCsqlFromParameters(serviceAreaNumbers, fromDate, toDate, dateColName);

            var pq = QueryHelpers.ParseQuery(WebUtility.HtmlDecode(Request.QueryString.Value));

            string filter = "";
            if (pq.ContainsKey(ExportQuery.CqlFilter))
            {
                var orgFilter = pq[ExportQuery.CqlFilter].First();
                pq.Remove(ExportQuery.CqlFilter);
                filter = $"{saCql} and ({orgFilter})";
                pq.Add(ExportQuery.CqlFilter, filter);
            }
            else
            {
                filter = saCql;
                pq.Add(ExportQuery.CqlFilter, filter);
            }

            pq.Add(ExportQuery.OutputFormat, outputFormat);

            pq.Remove(ExportQuery.ServiceAreas);
            pq.Remove(ExportQuery.FromDate);
            pq.Remove(ExportQuery.ToDate);
            pq.Remove(ExportQuery.Format);

            if (endpointConfigName == ExportQueryEndpointConfigName.WMS)
            {
                pq.Add(ExportQuery.Layers, pq[ExportQuery.TypeName]);
                pq.Remove(ExportQuery.TypeName);
            }

            if (count)
            {
                pq.Add(ExportQuery.Count, "1");
                pq.Remove(ExportQuery.OutputFormat);
                pq.Add(ExportQuery.OutputFormat, "application/json");
            }

            var pqkv = new List<KeyValuePair<string, string>>();
            foreach (var kv in pq)
            {
                pqkv.Add(new KeyValuePair<string, string>(kv.Key, kv.Value.First()));
            }

            var bq = new QueryBuilder(pqkv);

            return bq.ToQueryString().Value.RemoveQuestionMark();
        }

        private string BuildCsqlFromParameters(decimal[] serviceAreaNumbers, DateTime fromDate, DateTime toDate, string dateColName)
        {
            var csql = new StringBuilder();
            var fromDt = fromDate.ToString("yyyy-MM-dd");
            var toDt = toDate.ToString("yyyy-MM-dd");

            csql.Append($"({dateColName} BETWEEN {fromDt} AND {toDt} AND ");

            csql.Append("SERVICE_AREA IN (");

            foreach (var serviceAreaNumber in serviceAreaNumbers)
            {
                csql.Append($"{(int)serviceAreaNumber},");
            }

            return csql.ToString().TrimEnd(',') + "))";
        }

        private string GetDateColName(string typeName)
        {
            typeName = typeName.ToLowerInvariant();

            switch (typeName)
            {
                case "hmr:hmr_work_report_vw":
                    return DateColNames.EndDate;
                case "hmr:hmr_wildlife_report_vw":
                    return DateColNames.AccidentDate;
                case "hmr:hmr_rockfall_report_vw":
                    return DateColNames.RockfallDate;
                default:
                    return null;
            }
        }
    }
}
