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
    public interface IOasService
    {
        Task<bool> IsPointOnRfiSegment(int tolerance, decimal longitude, decimal latitude, string rfiSegment);
        Task<Line> GetLineFromOffsetMeasuerOnRfiSegment(string rfiSegment, decimal start, decimal end);
        Task<decimal> GetOffsetMeasureFromPointAndRfiSegment(decimal longitude, decimal latitude, string rfiSegment);
        Task<Point> GetPointFromOffsetMeasureOnRfiSegment(string rfiSegment, decimal offset);
    }
    public class OasService : IOasService
    {
        private HttpClient _client;
        private OasQueries _queries;
        private IApi _api;
        private const string _path = "ogs-geoV06/wfs?";

        public OasService(HttpClient client, IApi api)
        {
            _client = client;
            _api = api;
            _queries = new OasQueries();
        }

        public async Task<bool> IsPointOnRfiSegment(int tolerance, decimal longitude, decimal latitude, string rfiSegment)
        {
            var body = string.Format(_queries.PointOnRfiSegQuery, tolerance, longitude, latitude, rfiSegment);

            var contents = await _api.Post(_client, _path, body);

            var features = JsonSerializer.Deserialize<FeatureCollection<decimal[]>>(contents);

            return features.numberMatched > 0;
        }

        public async Task<Line> GetLineFromOffsetMeasuerOnRfiSegment(string rfiSegment, decimal start, decimal end)
        {
            var query = _path + string.Format(_queries.LineFromOffsetMeasureOnRfiSeg, rfiSegment, start, end);

            var content = await _api.Get(_client, query);

            var features = JsonSerializer.Deserialize<FeatureCollection<decimal[][]>>(content);

            if (features.totalFeatures == 0) return null;

            return new Line(features.features[0].geometry.coordinates);
        }

        public async Task<decimal> GetOffsetMeasureFromPointAndRfiSegment(decimal longitude, decimal latitude, string rfiSegment)
        {
            var query = _path + string.Format(_queries.OffsetMeasureFromPointAndRfiSeg, longitude, latitude, rfiSegment);

            var content = await _api.Get(_client, query);

            var features = JsonSerializer.Deserialize<FeatureCollection<decimal[]>>(content);

            if (features.totalFeatures == 0) return 0;

            return Convert.ToDecimal(features.features[0].properties.MEASURE);
        }

        public async Task<Point> GetPointFromOffsetMeasureOnRfiSegment(string rfiSegment, decimal offset)
        {
            var query = _path + string.Format(_queries.PointFromOffsetMeasureOnRfiSeg, rfiSegment, offset);

            var content = await _api.Get(_client, query);

            var features = JsonSerializer.Deserialize<FeatureCollection<decimal[]>>(content);

            if (features.totalFeatures == 0) return null;

            return new Point(features.features[0].geometry.coordinates);
        }
    }
}
