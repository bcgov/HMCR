using Hmcr.Chris.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hmcr.Chris
{
    public interface IInventoryApi
    {
        Task<List<SurfaceType>> GetSurfaceTypeAssociatedWithLine(string lineStringCoordinates);
        Task<SurfaceType> GetSurfaceTypeAssociatedWithPoint(string lineStringCoordinates);

        Task<List<MaintenanceClass>> GetMaintenanceClassesAssociatedWithLine(string lineStringCoordinates);
        Task<MaintenanceClass> GetMaintenanceClassesAssociatedWithPoint(string lineStringCoordinates);
        Task<HighwayProfile> GetHighwayProfileAssociatedWithPoint(string lineStringCoordinates);
        Task<List<HighwayProfile>> GetHighwayProfileAssociatedWithLine(string lineStringCoordinates);
        Task<Guardrail> GetGuardrailAssociatedWithPoint(string lineStringCoordinates);
        Task<List<Guardrail>> GetGuardrailAssociatedWithLine(string lineStringCoordinates);
        Task<List<Structure>> GetBridgeStructure(string rfiSegment);
    }

    public class InventoryApi : IInventoryApi
    {
        private HttpClient _client;
        private InventoryQueries _queries;
        private IApi _api;
        private string _path;
        private ILogger<IInventoryApi> _logger;

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
            public const string GR_ASSOC_WITH_LINE = "GR_ASSOCIATED_WITH_POINT";
            public const string GR_ASSOC_WITH_POINT = "GR_ASSOCIATED_WITH_LINE";
        }

        public InventoryApi(HttpClient client, IApi api, IConfiguration config, ILogger<IInventoryApi> logger)
        {
            _client = client;
            _queries = new InventoryQueries();
            _api = api;
            _path = config.GetValue<string>("CHRIS:OASPath");
            _logger = logger;
        }

        public async Task<SurfaceType> GetSurfaceTypeAssociatedWithPoint(string lineStringCoordinates)
        {
            var body = "";
            var contents = "";

            try
            {
                body = string.Format(_queries.InventoryAssocWithPointQuery, lineStringCoordinates, InventoryQueryTypeName.SURF_ASSOC_WITH_POINT);

                contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                SurfaceType surfaceType = new SurfaceType();
                if (results.features.Length > 0)
                {
                    surfaceType.Type = results.features[0].properties.SURFACE_TYPE;
                }

                return surfaceType;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetSurfaceTypeAssociatedWithPoint: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<List<SurfaceType>> GetSurfaceTypeAssociatedWithLine(string lineStringCoordinates)
        {
            var body = "";
            var contents = "";

            try
            {
                body = string.Format(_queries.InventoryAssocWithLineQuery, lineStringCoordinates, InventoryQueryTypeName.SURF_ASSOC_WITH_LINE);

                contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                var surfaceTypes = new List<SurfaceType>();

                foreach (var feature in results.features)
                {
                    SurfaceType surfaceType = new SurfaceType();
                    surfaceType.Length = feature.properties.CLIPPED_LENGTH_KM;
                    surfaceType.Type = feature.properties.SURFACE_TYPE;

                    surfaceTypes.Add(surfaceType);
                }

                return surfaceTypes;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetSurfaceTypeAssociatedWithLine: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<List<MaintenanceClass>> GetMaintenanceClassesAssociatedWithLine(string lineStringCoordinates)
        {
            var body = "";
            var contents = "";

            try
            {
                body = string.Format(_queries.InventoryAssocWithLineQuery, lineStringCoordinates, InventoryQueryTypeName.MC_ASSOC_WITH_LINE);

                contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                var maintenanceClasses = new List<MaintenanceClass>();

                foreach (var feature in results.features)
                {
                    MaintenanceClass maintenanceClass = new MaintenanceClass();
                    maintenanceClass.Length = feature.properties.CLIPPED_LENGTH_KM;
                    maintenanceClass.SummerRating = feature.properties.SUMMER_CLASS_RATING;
                    maintenanceClass.WinterRating = feature.properties.WINTER_CLASS_RATING;

                    maintenanceClasses.Add(maintenanceClass);
                }

                return maintenanceClasses;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetMaintenanceClassesAssociatedWithLine: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<MaintenanceClass> GetMaintenanceClassesAssociatedWithPoint(string lineStringCoordinates)
        {
            var body = "";
            var contents = "";

            try
            {
                body = string.Format(_queries.InventoryAssocWithPointQuery, lineStringCoordinates, InventoryQueryTypeName.MC_ASSOC_WITH_POINT);

                contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                MaintenanceClass maintenanceClass = new MaintenanceClass();
                if (results.features.Length > 0)
                {
                    maintenanceClass.SummerRating = results.features[0].properties.SUMMER_CLASS_RATING;
                    maintenanceClass.WinterRating = results.features[0].properties.WINTER_CLASS_RATING;
                }

                return maintenanceClass;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetMaintenanceClassesAssociatedWithPoint: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<HighwayProfile> GetHighwayProfileAssociatedWithPoint(string lineStringCoordinates)
        {
            var body = "";
            var contents = "";

            try
            {
                body = string.Format(_queries.InventoryAssocWithPointQuery, lineStringCoordinates, InventoryQueryTypeName.HP_ASSOC_WITH_POINT);

                contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                HighwayProfile highwayProfile = new HighwayProfile();
                if (results.features.Length > 0)
                {
                    highwayProfile.NumberOfLanes = results.features[0].properties.NUMBER_OF_LANES;
                    highwayProfile.DividedHighwayFlag = results.features[0].properties.DIVIDED_HIGHWAY_FLAG;
                }

                return highwayProfile;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetHighwayProfileAssociatedWithPoint: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<List<HighwayProfile>> GetHighwayProfileAssociatedWithLine(string lineStringCoordinates)
        {
            var body = "";
            var contents = "";

            try
            {
                body = string.Format(_queries.InventoryAssocWithLineQuery, lineStringCoordinates, InventoryQueryTypeName.HP_ASSOC_WITH_LINE);

                contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                var highwayProfiles = new List<HighwayProfile>();

                foreach (var feature in results.features)
                {
                    HighwayProfile highwayProfile = new HighwayProfile();
                    highwayProfile.Length = feature.properties.CLIPPED_LENGTH_KM;
                    highwayProfile.NumberOfLanes = feature.properties.NUMBER_OF_LANES;
                    highwayProfile.DividedHighwayFlag = feature.properties.DIVIDED_HIGHWAY_FLAG;

                    highwayProfiles.Add(highwayProfile);
                }

                return highwayProfiles;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetHighwayProfileAssociatedWithLine: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<Guardrail> GetGuardrailAssociatedWithPoint(string lineStringCoordinates)
        {
            var body = "";
            var contents = "";

            try
            {
                body = string.Format(_queries.InventoryAssocWithPointQuery, lineStringCoordinates, InventoryQueryTypeName.HP_ASSOC_WITH_POINT);

                contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                Guardrail guardrail = new Guardrail();
                if (results.features.Length > 0)
                {
                    guardrail.GuardrailType = results.features[0].properties.GUARDRAIL_TYPE;
                }

                return guardrail;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetGuardrailAssociatedWithPoint: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<List<Guardrail>> GetGuardrailAssociatedWithLine(string lineStringCoordinates)
        {
            var body = "";
            var contents = "";

            try
            {
                body = string.Format(_queries.InventoryAssocWithLineQuery, lineStringCoordinates, InventoryQueryTypeName.HP_ASSOC_WITH_LINE);

                contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                var guardrails = new List<Guardrail>();

                foreach (var feature in results.features)
                {
                    Guardrail guardrail = new Guardrail();
                    guardrail.Length = feature.properties.CLIPPED_LENGTH_KM;
                    guardrail.GuardrailType = feature.properties.GUARDRAIL_TYPE;

                    guardrails.Add(guardrail);
                }

                return guardrails;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetGuardrailAssociatedWithLine: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<List<Structure>> GetBridgeStructure(string rfiSegment)
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
                    structure.StructureType = feature.properties.IIT_INV_TYPE;  //this may possibly be feature.properties.BMIS_STRUCTURE_TYPE
                    structure.BeginKM = feature.properties.BEGIN_KM;
                    structure.EndKM = feature.properties.END_KM;
                    structure.Length = feature.properties.LENGTH_KM;

                    structures.Add(structure);
                }

                return structures;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - GetRfiSegmentDetailAsync: {query} - {content}");
                throw ex;
            }
        }
    }
}
