using Hmcr.Chris.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hmcr.Chris
{
    public interface IInventoryApi
    {
        Task<List<SurfaceType>> GetSurfaceTypeAssociatedWithLine(NetTopologySuite.Geometries.Geometry geometry);
        Task<SurfaceType> GetSurfaceTypeAssociatedWithPoint(NetTopologySuite.Geometries.Geometry geometry);
        Task<List<MaintenanceClass>> GetMaintenanceClassesAssociatedWithLine(NetTopologySuite.Geometries.Geometry geometry);
        Task<MaintenanceClass> GetMaintenanceClassesAssociatedWithPoint(NetTopologySuite.Geometries.Geometry geometry);
        Task<HighwayProfile> GetHighwayProfileAssociatedWithPoint(NetTopologySuite.Geometries.Geometry geometry);
        Task<List<HighwayProfile>> GetHighwayProfileAssociatedWithLine(NetTopologySuite.Geometries.Geometry geometry);
        Task<Guardrail> GetGuardrailAssociatedWithPoint(NetTopologySuite.Geometries.Geometry geometry);
        Task<List<Guardrail>> GetGuardrailAssociatedWithLine(NetTopologySuite.Geometries.Geometry geometry);
        Task<List<Structure>> GetStructuresOnRFISegment(string rfiSegment);
        Task<List<RestArea>> GetRestAreasOnRFISegment(string rfiSegment);
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

        /// <summary>
        /// Utility function takes an array of coordinates and builds them 
        /// into a string with a limit of 250 coordinate pairs in a group. 
        /// The group is placed into a string array that is then returned 
        /// for processing.
        /// This is to deal with https://jira.th.gov.bc.ca/browse/HMCR-871 
        /// in which GeoServer only accepts a max of 500 coordinate pairs
        /// and also ensures we don't hit the 120sec timeout limit or 
        /// MAX post size.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        private List<string> BuildGeometryString(Coordinate[] coordinates)
        {
            var geometryGroup = new List<string>();
            var geometryLineString = "";
            var coordinateCount = 0;

            foreach (Coordinate coordinate in coordinates)
            {
                geometryLineString += coordinate.X + "\\," + coordinate.Y + "\\,";
                coordinateCount++;
                if (coordinateCount == 250 || (coordinate == coordinates.Last()))
                {
                    geometryGroup.Add(geometryLineString.Substring(0, geometryLineString.Length - 2));
                    geometryLineString = "";
                    coordinateCount = 0;
                }
            }

            //check the last item to see if it's a single since the GeoServer queries expect
            // at least 2 pairs we'll need to make some adjustments if there is a group with 1 pair
            // this won't exceed the paramter limit since it's 1000 (500 pairs). See HMCR-909
            var count = geometryGroup.Count();
            if (geometryGroup[count - 1].Count(g => g == ',') == 1)
            {
                var lastGroup = geometryGroup[count - 1];
                var secondLastGroup = geometryGroup[count - 2];
                secondLastGroup += "\\," + lastGroup;   //append the previously last group to the 2nd last group text
                geometryGroup[count - 2] = secondLastGroup; //update the second last group
                geometryGroup.RemoveAt(count - 1);  //remove the last group
            }

            return geometryGroup;
        }

        public async Task<SurfaceType> GetSurfaceTypeAssociatedWithPoint(NetTopologySuite.Geometries.Geometry geometry)
        {
            var body = "";
            var contents = "";
            SurfaceType surfaceType = new SurfaceType();

            try
            {
                var geometryGroup = BuildGeometryString(geometry.Coordinates);

                foreach (var lineStringCoordinates in geometryGroup)
                {
                    body = string.Format(_queries.InventoryAssocWithPointQuery, InventoryParamType.POINT_COORDINATE, lineStringCoordinates, InventoryQueryTypeName.SURF_ASSOC_WITH_POINT);

                    contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                    var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                    if (results.features.Length > 0)
                    {
                        surfaceType.Type = results.features[0].properties.SURFACE_TYPE;
                    }
                }

                return surfaceType;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetSurfaceTypeAssociatedWithPoint: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<List<SurfaceType>> GetSurfaceTypeAssociatedWithLine(NetTopologySuite.Geometries.Geometry geometry)
        {
            var body = "";
            var contents = "";
            var surfaceTypes = new List<SurfaceType>();

            try
            {
                var geometryGroup = BuildGeometryString(geometry.Coordinates);

                foreach (var lineStringCoordinates in geometryGroup)
                {
                    body = string.Format(_queries.InventoryAssocWithLineQuery, InventoryParamType.LINE_COORDINATE, lineStringCoordinates, InventoryQueryTypeName.SURF_ASSOC_WITH_LINE);

                    contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                    var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                    foreach (var feature in results.features)
                    {
                        SurfaceType surfaceType = new SurfaceType();
                        surfaceType.Length = feature.properties.CLIPPED_LENGTH_KM;
                        surfaceType.Type = feature.properties.SURFACE_TYPE;

                        surfaceTypes.Add(surfaceType);
                    }
                }

                return surfaceTypes;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetSurfaceTypeAssociatedWithLine: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<List<MaintenanceClass>> GetMaintenanceClassesAssociatedWithLine(NetTopologySuite.Geometries.Geometry geometry)
        {
            var body = "";
            var contents = "";
            var maintenanceClasses = new List<MaintenanceClass>();

            try
            {
                var geometryGroup = BuildGeometryString(geometry.Coordinates);

                foreach (var lineStringCoordinates in geometryGroup)
                {
                    body = string.Format(_queries.InventoryAssocWithLineQuery, InventoryParamType.LINE_COORDINATE, lineStringCoordinates, InventoryQueryTypeName.MC_ASSOC_WITH_LINE);

                    contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                    var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                    foreach (var feature in results.features)
                    {
                        MaintenanceClass maintenanceClass = new MaintenanceClass();
                        maintenanceClass.Length = feature.properties.CLIPPED_LENGTH_KM;
                        maintenanceClass.SummerRating = feature.properties.SUMMER_CLASS_RATING;
                        maintenanceClass.WinterRating = feature.properties.WINTER_CLASS_RATING;

                        maintenanceClasses.Add(maintenanceClass);
                    }
                }

                return maintenanceClasses;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetMaintenanceClassesAssociatedWithLine: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<MaintenanceClass> GetMaintenanceClassesAssociatedWithPoint(NetTopologySuite.Geometries.Geometry geometry)
        {
            var body = "";
            var contents = "";
            MaintenanceClass maintenanceClass = new MaintenanceClass();

            try
            {
                var geometryGroup = BuildGeometryString(geometry.Coordinates);

                foreach (var lineStringCoordinates in geometryGroup)
                {
                    body = string.Format(_queries.InventoryAssocWithPointQuery, InventoryParamType.POINT_COORDINATE, lineStringCoordinates, InventoryQueryTypeName.MC_ASSOC_WITH_POINT);

                    contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                    var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                    if (results.features.Length > 0)
                    {
                        maintenanceClass.SummerRating = results.features[0].properties.SUMMER_CLASS_RATING;
                        maintenanceClass.WinterRating = results.features[0].properties.WINTER_CLASS_RATING;
                    }
                }

                return maintenanceClass;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetMaintenanceClassesAssociatedWithPoint: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<HighwayProfile> GetHighwayProfileAssociatedWithPoint(NetTopologySuite.Geometries.Geometry geometry)
        {
            var body = "";
            var contents = "";
            HighwayProfile highwayProfile = new HighwayProfile();

            try
            {
                var geometryGroup = BuildGeometryString(geometry.Coordinates);

                foreach (var lineStringCoordinates in geometryGroup)
                {
                    body = string.Format(_queries.InventoryAssocWithPointQuery, InventoryParamType.POINT_COORDINATE, lineStringCoordinates, InventoryQueryTypeName.HP_ASSOC_WITH_POINT);

                    contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                    var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                    
                    if (results.features.Length > 0)
                    {
                        highwayProfile.NumberOfLanes = results.features[0].properties.NUMBER_OF_LANES;
                        highwayProfile.DividedHighwayFlag = results.features[0].properties.DIVIDED_HIGHWAY_FLAG;
                    }
                }

                return highwayProfile;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetHighwayProfileAssociatedWithPoint: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<List<HighwayProfile>> GetHighwayProfileAssociatedWithLine(NetTopologySuite.Geometries.Geometry geometry)
        {
            var body = "";
            var contents = "";
            var highwayProfiles = new List<HighwayProfile>();

            try
            {
                var geometryGroup = BuildGeometryString(geometry.Coordinates);

                foreach (var lineStringCoordinates in geometryGroup)
                {
                    body = string.Format(_queries.InventoryAssocWithLineQuery, InventoryParamType.LINE_COORDINATE, lineStringCoordinates, InventoryQueryTypeName.HP_ASSOC_WITH_LINE);

                    contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                    var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                    foreach (var feature in results.features)
                    {
                        HighwayProfile highwayProfile = new HighwayProfile();
                        highwayProfile.Length = feature.properties.CLIPPED_LENGTH_KM;
                        highwayProfile.NumberOfLanes = feature.properties.NUMBER_OF_LANES;
                        highwayProfile.DividedHighwayFlag = feature.properties.DIVIDED_HIGHWAY_FLAG;

                        highwayProfiles.Add(highwayProfile);
                    }
                }

                return highwayProfiles;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetHighwayProfileAssociatedWithLine: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<Guardrail> GetGuardrailAssociatedWithPoint(NetTopologySuite.Geometries.Geometry geometry)
        {
            var body = "";
            var contents = "";
            Guardrail guardrail = new Guardrail();

            try
            {
                var geometryGroup = BuildGeometryString(geometry.Coordinates);

                foreach (var lineStringCoordinates in geometryGroup)
                {
                    body = string.Format(_queries.InventoryAssocWithPointQuery, InventoryParamType.POINT_COORDINATE, lineStringCoordinates, InventoryQueryTypeName.GR_ASSOC_WITH_POINT);

                    contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                    var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                    if (results.features.Length > 0)
                    {
                        guardrail.GuardrailType = results.features[0].properties.GUARDRAIL_TYPE;
                        guardrail.CrossSectionPosition = results.features[0].properties.IIT_X_SECT;
                    }
                }

                return guardrail;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetGuardrailAssociatedWithPoint: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<List<Guardrail>> GetGuardrailAssociatedWithLine(NetTopologySuite.Geometries.Geometry geometry)
        {
            var body = "";
            var contents = "";
            var guardrails = new List<Guardrail>();

            try
            {
                var geometryGroup = BuildGeometryString(geometry.Coordinates);

                foreach (var lineStringCoordinates in geometryGroup)
                {
                    body = string.Format(_queries.InventoryAssocWithLineQuery, InventoryParamType.LINE_COORDINATE, lineStringCoordinates, InventoryQueryTypeName.GR_ASSOC_WITH_LINE);

                    contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                    var results = JsonSerializer.Deserialize<FeatureCollection<object>>(contents);

                    foreach (var feature in results.features)
                    {
                        Guardrail guardrail = new Guardrail();
                        guardrail.Length = feature.properties.CLIPPED_LENGTH_KM;
                        guardrail.GuardrailType = feature.properties.GUARDRAIL_TYPE;
                        guardrail.CrossSectionPosition = feature.properties.IIT_X_SECT;

                        guardrails.Add(guardrail);
                    }
                }

                return guardrails;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception - GetGuardrailAssociatedWithLine: {body} - {contents}");
                throw ex;
            }
        }

        public async Task<List<Structure>> GetStructuresOnRFISegment(string rfiSegment)
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
                _logger.LogError($"Exception - GetStructuresOnRFISegment: {query} - {content}");
                throw ex;
            }
        }

        public async Task<List<RestArea>> GetRestAreasOnRFISegment(string rfiSegment)
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
                _logger.LogError($"Exception - GetRestAreasOnRFISegment: {query} - {content}");
                throw ex;
            }
        }
    }
}
