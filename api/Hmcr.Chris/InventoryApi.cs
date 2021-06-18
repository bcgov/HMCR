using Hmcr.Chris.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GJFeature = GeoJSON.Net.Feature;  // use an alias since Feature exists in HttpClients.Models
using System.Xml;

namespace Hmcr.Chris
{
    public interface IInventoryApi
    {
        Task<List<Structure>> GetStructuresOnRFISegment(string rfiSegment, string recordNumber);
        Task<List<RestArea>> GetRestAreasOnRFISegment(string rfiSegment, string recordNumber);

        Task<(GJFeature.FeatureCollection featureCollection, string errorMsg)> GetSurfaceType(string rfiSegmentName, string recordNumber);
        Task<(GJFeature.FeatureCollection featureCollection, string errorMsg)> GetMaintenanceClass(string rfiSegmentName, string recordNumber);
        Task<(GJFeature.FeatureCollection featureCollection, string errorMsg)> GetHighwayProfile(string rfiSegmentName, string recordNumber);
        Task<(GJFeature.FeatureCollection featureCollection, string errorMsg)> GetGuardrail(string rfiSegmentName, string recordNumber);
    }

    public class InventoryApi : IInventoryApi
    {
        private HttpClient _client;
        private InventoryQueries _queries;
        private IApi _api;
        private string _path;
        private ILogger<IInventoryApi> _logger;
        private const string _maxFeatures = "500";

        /// <summary>
        /// Chris Query typeName values, used to call the Get Inventory
        /// Assoc with Work Activity Query.
        /// </summary>
        public static class InventoryQueryTypeName
        {
            public const string SURF_ASSOC_WITH_LINE = "SURF_ASSOCIATED_WITH_LINE";
            public const string SURF_ASSOC_WITH_POINT = "SURF_ASSOCIATED_WITH_POINT";
            public const string MC_ASSOC_WITH_LINE = "MC_ASSOCIATED_WITH_LINE";
            public const string MC_ASSOC_WITH_POINT = "MC_ASSOCIATED_WITH_POINT";
            public const string HP_ASSOC_WITH_LINE = "HP_ASSOCIATED_WITH_LINE";
            public const string HP_ASSOC_WITH_POINT = "HP_ASSOCIATED_WITH_POINT";
            public const string GR_ASSOC_WITH_LINE = "GR_ASSOCIATED_WITH_LINE";
            public const string GR_ASSOC_WITH_POINT = "GR_ASSOCIATED_WITH_POINT";
        }
        public static class InventoryParamType
        {
            public const string POINT_COORDINATE = "coordinate";
            public const string LINE_COORDINATE = "coordinates";
        }

        public InventoryApi(HttpClient client, IApi api, IConfiguration config, ILogger<IInventoryApi> logger)
        {
            _client = client;
            _queries = new InventoryQueries();
            _api = api;
            _path = config.GetValue<string>("CHRIS:OASPath");
            _logger = logger;
        }

        public async Task<(GJFeature.FeatureCollection featureCollection, string errorMsg)> GetSurfaceType(string rfiSegmentName, string recordNumber)
        {
            var query = "";
            var content = "";
            string errText = "";

            GJFeature.FeatureCollection featureCollection = null;

            try
            {
                query = _path + string.Format(_queries.InventoryAssociatedWithRFI, GeoServerEndpoint.SurfaceType, _maxFeatures, rfiSegmentName);
                content = await (await _api.GetWithRetry(_client, query)).Content.ReadAsStringAsync();

                if (content.StartsWith("<"))
                {
                    //we got xml back.. it's likely an error, we should handle these
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(content);
                    errText = xdoc.InnerText.Replace("\n", "").Trim();
                } else
                {
                    featureCollection = ParseJSONToFeatureCollection(content);
                }

                return (featureCollection, errText);
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetSurfaceType({rfiSegmentName}, {recordNumber}): {query} - {content}");
                throw ex;
            }
        }

        public async Task<(GJFeature.FeatureCollection featureCollection, string errorMsg)> GetMaintenanceClass(string rfiSegmentName, string recordNumber)
        {
            var query = "";
            var content = "";
            string errText = "";
            GJFeature.FeatureCollection featureCollection = null;

            try
            {
                query = _path + string.Format(_queries.InventoryAssociatedWithRFI, GeoServerEndpoint.MaintenanceClass, _maxFeatures, rfiSegmentName);
                content = await (await _api.GetWithRetry(_client, query)).Content.ReadAsStringAsync();
                
                if (content.StartsWith("<"))
                {
                    //we got xml back.. it's likely an error, we should handle these
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(content);
                    errText = xdoc.InnerText.Replace("\n", "").Trim();
                }
                else
                {
                    featureCollection = ParseJSONToFeatureCollection(content);
                }

                return (featureCollection, errText);
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetMaintenanceClass({rfiSegmentName}, {recordNumber}): {query} - {content}");
                throw ex;
            }
        }

        public async Task<(GJFeature.FeatureCollection featureCollection, string errorMsg)> GetHighwayProfile(string rfiSegmentName, string recordNumber)
        {
            var query = "";
            var content = "";
            string errText = "";
            GJFeature.FeatureCollection featureCollection = null;
            
            try
            {
                query = _path + string.Format(_queries.InventoryAssociatedWithRFI, GeoServerEndpoint.HighwayProfile, _maxFeatures, rfiSegmentName);
                content = await (await _api.GetWithRetry(_client, query)).Content.ReadAsStringAsync();

                if (content.StartsWith("<"))
                {
                    //we got xml back.. it's likely an error, we should handle these
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(content);
                    errText = xdoc.InnerText.Replace("\n", "").Trim();
                }
                else
                {
                    featureCollection = ParseJSONToFeatureCollection(content);
                }

                return (featureCollection, errText);
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetMaintenanceClass({rfiSegmentName}, {recordNumber}): {query} - {content}");
                throw ex;
            }
        }

        public async Task<(GJFeature.FeatureCollection featureCollection, string errorMsg)> GetGuardrail(string rfiSegmentName, string recordNumber)
        {
            var query = "";
            var content = "";
            string errText = "";
            GJFeature.FeatureCollection featureCollection = null;

            try
            {
                query = _path + string.Format(_queries.InventoryAssociatedWithRFI, GeoServerEndpoint.Guardrail, _maxFeatures, rfiSegmentName);
                content = await (await _api.GetWithRetry(_client, query)).Content.ReadAsStringAsync();

                if (content.StartsWith("<"))
                {
                    //we got xml back.. it's likely an error, we should handle these
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(content);
                    errText = xdoc.InnerText.Replace("\n", "").Trim();
                }
                else
                {
                    featureCollection = ParseJSONToFeatureCollection(content);
                }

                return (featureCollection, errText);
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetGuardrail({rfiSegmentName}, {recordNumber}): {query} - {content}");
                throw ex;
            }
        }

        public async Task<List<Structure>> GetStructuresOnRFISegment(string rfiSegment, string recordNumber)
        {
            var query = "";
            var content = "";

            try
            {
                query = _path + string.Format(_queries.StructureOnRfiSegment, rfiSegment);

                content = await (await _api.GetWithRetry(_client, query)).Content.ReadAsStringAsync();

                var results = JsonSerializer.Deserialize<FeatureCollection<object>>(content);

                var structures = new List<Structure>();

                foreach (var feature in results.features)
                {
                    Structure structure = new Structure();
                    structure.StructureType = feature.properties.BMIS_STRUCTURE_TYPE;
                    // need to apply trim start '0' to strip leading zeros, if the structure number in the typed row
                    //  had no alpha chars the CSV parse casts it as a number (ie. 00525 = 525)
                    structure.StructureNumber = feature.properties.BMIS_STRUCTURE_NO.TrimStart('0');
                    structure.BeginKM = feature.properties.BEGIN_KM;
                    structure.EndKM = feature.properties.END_KM;
                    structure.Length = feature.properties.LENGTH_KM;

                    structures.Add(structure);
                }

                return structures;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - GetStructuresOnRFISegment({recordNumber}): {query} - {content}");
                throw ex;
            }
        }

        public async Task<List<RestArea>> GetRestAreasOnRFISegment(string rfiSegment, string recordNumber)
        {
            var query = "";
            var content = "";

            try
            {
                query = _path + string.Format(_queries.RestAreaOnRfiSegment, rfiSegment);

                content = await (await _api.GetWithRetry(_client, query)).Content.ReadAsStringAsync();

                var results = JsonSerializer.Deserialize<FeatureCollection<object>>(content);

                var restAreas = new List<RestArea>();

                foreach (var feature in results.features)
                {
                    RestArea restArea = new RestArea();
                    restArea.SiteNumber = feature.properties.REST_AREA_NUMBER;
                    restArea.LocationKM = feature.properties.LOC_KM;
                    restArea.Class = feature.properties.REST_AREA_CLASS;

                    restAreas.Add(restArea);
                }

                return restAreas;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - GetRestAreasOnRFISegment({recordNumber}): {query} - {content}");
                throw ex;
            }
        }

        public static GJFeature.FeatureCollection ParseJSONToFeatureCollection(string jsonContent)
        {
            //create NTS JSON reader
            var reader = new GeoJsonReader();

            // pass the geoJSON to the reader and cast return to FeatureCollection
            var fc = reader.Read<GJFeature.FeatureCollection>(jsonContent);

            // fail out if no featureCollection
            if (fc == null)
                return null;
            else
                return fc;
        }
    }
}
