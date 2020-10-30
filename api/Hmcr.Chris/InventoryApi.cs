﻿using Hmcr.Chris.Models;
using Microsoft.Extensions.Configuration;
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
    }

    public class InventoryApi : IInventoryApi
    {
        private HttpClient _client;
        private InventoryQueries _queries;
        private IApi _api;
        private string _path;

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

        public InventoryApi(HttpClient client, IApi api, IConfiguration config)
        {
            _client = client;
            _queries = new InventoryQueries();
            _api = api;
            _path = config.GetValue<string>("CHRIS:OASPath");
        }

        public async Task<SurfaceType> GetSurfaceTypeAssociatedWithPoint(string lineStringCoordinates)
        {
            var body = string.Format(_queries.InventoryAssocWithPointQuery, lineStringCoordinates, InventoryQueryTypeName.SURF_ASSOC_WITH_POINT);

            var contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

            var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

            SurfaceType surfaceType = new SurfaceType();
            if (results.features.Length > 0)
            {
                surfaceType.Type = results.features[0].properties.SURFACE_TYPE;
            }

            return surfaceType;
        }

        public async Task<List<SurfaceType>> GetSurfaceTypeAssociatedWithLine(string lineStringCoordinates)
        {
            var body = string.Format(_queries.InventoryAssocWithLineQuery, lineStringCoordinates, InventoryQueryTypeName.SURF_ASSOC_WITH_LINE);

            var contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

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

        public async Task<List<MaintenanceClass>> GetMaintenanceClassesAssociatedWithLine(string lineStringCoordinates)
        {
            var body = string.Format(_queries.InventoryAssocWithLineQuery, lineStringCoordinates, InventoryQueryTypeName.MC_ASSOC_WITH_LINE);

            var contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

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

        public async Task<MaintenanceClass> GetMaintenanceClassesAssociatedWithPoint(string lineStringCoordinates)
        {
            var body = string.Format(_queries.InventoryAssocWithPointQuery, lineStringCoordinates, InventoryQueryTypeName.MC_ASSOC_WITH_POINT);

            var contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

            var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

            MaintenanceClass maintenanceClass = new MaintenanceClass();
            if (results.features.Length > 0)
            {
                maintenanceClass.SummerRating = results.features[0].properties.SUMMER_CLASS_RATING;
                maintenanceClass.WinterRating = results.features[0].properties.WINTER_CLASS_RATING;
            }

            return maintenanceClass;
        }

        public async Task<HighwayProfile> GetHighwayProfileAssociatedWithPoint(string lineStringCoordinates)
        {
            var body = string.Format(_queries.InventoryAssocWithPointQuery, lineStringCoordinates, InventoryQueryTypeName.HP_ASSOC_WITH_POINT);

            var contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

            var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

            HighwayProfile highwayProfile = new HighwayProfile();
            if (results.features.Length > 0)
            {
                highwayProfile.NumberOfLanes = results.features[0].properties.NUMBER_OF_LANES;
                highwayProfile.DividedHighwayFlag = results.features[0].properties.DIVIDED_HIGHWAY_FLAG;
            }

            return highwayProfile;
        }

        public async Task<List<HighwayProfile>> GetHighwayProfileAssociatedWithLine(string lineStringCoordinates)
        {
            var body = string.Format(_queries.InventoryAssocWithLineQuery, lineStringCoordinates, InventoryQueryTypeName.HP_ASSOC_WITH_LINE);

            var contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

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

        public async Task<Guardrail> GetGuardrailAssociatedWithPoint(string lineStringCoordinates)
        {
            var body = string.Format(_queries.InventoryAssocWithPointQuery, lineStringCoordinates, InventoryQueryTypeName.HP_ASSOC_WITH_POINT);

            var contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

            var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

            Guardrail guardrail = new Guardrail();
            if (results.features.Length > 0)
            {
                guardrail.GuardrailType = results.features[0].properties.GUARDRAIL_TYPE;
            }

            return guardrail;
        }

        public async Task<List<Guardrail>> GetGuardrailAssociatedWithLine(string lineStringCoordinates)
        {
            var body = string.Format(_queries.InventoryAssocWithLineQuery, lineStringCoordinates, InventoryQueryTypeName.HP_ASSOC_WITH_LINE);

            var contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

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
    }
}
