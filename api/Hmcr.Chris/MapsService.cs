using Hmcr.Chris.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hmcr.Chris
{
    public interface IMpasService 
    {
        Task<bool> IsPointWithinServiceAreaQuery(decimal longitude, decimal latitude, string serviceAreaNumber);
    }

    public class MapsService : IMpasService
    {
        private HttpClient _client;
        private MapsQueries _queries;
        private IApi _api;
        private const string _path = "geoV05/wfs?";

        public MapsService(HttpClient client, IApi api)
        {
            _client = client;
            _queries = new MapsQueries();
            _api = api;
        }

        public async Task<bool> IsPointWithinServiceAreaQuery(decimal longitude, decimal latitude, string serviceAreaNumber)
        {
            var body = string.Format(_queries.PointWithinServiceAreaQuery, longitude, latitude, serviceAreaNumber);

            var contents = await _api.Post(_client, _path, body);

            var features = JsonSerializer.Deserialize<FeatureCollection<decimal[]>>(contents);

            return features.numberMatched > 0;
        }
    }
}
