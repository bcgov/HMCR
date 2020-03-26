﻿using Hmcr.Api.Authorization;
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
using System;

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

        /// <summary>
        /// Search report records and export in the specified output format
        /// </summary>
        /// <param name="serviceAreas">1 ~ 28</param>
        /// <param name="typeName">hmr:HMR_WORK_REPORT_VW, hmr:HMR_WILDLIFE_REPORT_VW, hmr:HMR_ROCKFALL_REPORT_VW</param>
        /// <param name="outputFormat">csv, application/json, application/vnd.google-earth.kml+xml</param>
        /// <param name="fromDate">From date in yyyy-MM-dd format</param>
        /// <param name="toDate">To date in yyyy-MM-dd format</param>
        /// <returns></returns>
        [HttpGet("report", Name = "Export")]
        [RequiresPermission(Permissions.FileUploadRead)]        
        public async Task<IActionResult> ExportReport(string serviceAreas, string typeName, string outputFormat, DateTime fromDate, DateTime toDate)
        {
            var serviceAreaNumbers = serviceAreas.ToDecimalArray();

            if (serviceAreaNumbers.Length == 0)
            {
                serviceAreaNumbers = _currentUser.UserInfo.ServiceAreas.Select(x => x.ServiceAreaNumber).ToArray();
            }
            
            var invalidResult = ValidateQueryParameters(serviceAreaNumbers, typeName, outputFormat, fromDate, toDate);

            if (invalidResult != null)
            {
                return invalidResult;
            }

            var query = BuildQuery(serviceAreaNumbers, fromDate, toDate, GetDateColName(typeName)); 

            var responseMessage = await _exportApi.ExportReport(query);

            var (mimeType, fileName) = GetContentType(outputFormat);

            if (mimeType == null)
            {
                return ValidationUtils.GetValidationErrorResult(ControllerContext,
                    "Invalid output format", "Please include a valid output format in the query string.");
            }

            var bytes = await responseMessage.Content.ReadAsByteArrayAsync();

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                return File(bytes, responseMessage.Content.Headers.ContentType.ToString(), fileName);
            }
            else
            {
                return ValidationUtils.GetValidationErrorResult(ControllerContext,
                     (int)responseMessage.StatusCode, "Error from Geoserver", Encoding.UTF8.GetString(bytes));
            }
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

        private (string mimeType, string fileName) GetContentType(string outputFormat)
        {
            if (outputFormat == null)
            {
                return (null, null);
            }

            outputFormat = outputFormat.ToLowerInvariant();

            switch (outputFormat)
            {
                case "csv":
                    return ("text/csv;charset=UTF-8", "export.csv");
                case "application/json":
                    return ("application/json;charset=UTF-8", "export.json");
                case "application/vnd.google-earth.kml+xml":
                case "application/vnd.google-earth.kml xml":
                    return ("application/vnd.google-earth.kml+xml;charset=UTF-8", "export.kml");
                case "application/vnd.google-earth.kmz+xml":
                case "application/vnd.google-earth.kmz xml":
                    return ("application/vnd.google-earth.kmz+xml;charset=UTF-8", "export.kml");
                case "application/gml+xml;version=3.2":
                case "application/gml xml;version=3.2":
                    return ("application/gml+xml; version=3.2;charset=UTF-8", "export.gml");
                default:
                    return (null, null);
            }
        }

        private string BuildQuery(decimal[] serviceAreaNumbers, DateTime fromDate, DateTime toDate, string dateColName)
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

            pq.Remove(ExportQuery.ServiceAreas);
            pq.Remove(ExportQuery.FromDate);
            pq.Remove(ExportQuery.ToDate);

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

            foreach(var serviceAreaNumber in serviceAreaNumbers)
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
                    return DateColNames.ReportDate;
                default:
                    return null;
            }
        }
    }
}