using Hmcr.Chris.Models;
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

        //TODO: implement MC calls to CHRIS

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
            var body = string.Format(_queries.SurfaceTypeAssocWithPointQuery, lineStringCoordinates, InventoryQueryTypeName.SURF_ASSOC_WITH_POINT);

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
            var body = string.Format(_queries.SurfaceTypeAssocWithLineQuery, lineStringCoordinates, InventoryQueryTypeName.SURF_ASSOC_WITH_LINE);

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
    }
}
