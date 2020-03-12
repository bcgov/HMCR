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
    public interface IMapsApi
    {
        Task<bool> IsPointWithinServiceAreaQuery(decimal longitude, decimal latitude, string serviceAreaNumber);
    }

    public class MapsApi : IMapsApi
    {
        private HttpClient _client;
        private MapsQueries _queries;
        private IApi _api;
        private string _path;

        public MapsApi(HttpClient client, IApi api, IConfiguration config)
        {
            _client = client;
            _queries = new MapsQueries();
            _api = api;
            _path = config.GetValue<string>("CHRIS:MapPath");
        }

        public async Task<bool> IsPointWithinServiceAreaQuery(decimal longitude, decimal latitude, string serviceAreaNumber)
        {
            var body = string.Format(_queries.PointWithinServiceAreaQuery, longitude, latitude, serviceAreaNumber);

            var contents = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

            var features = JsonSerializer.Deserialize<FeatureCollection<decimal[]>>(contents);

            return features.numberMatched > 0;
        }
    }
}
