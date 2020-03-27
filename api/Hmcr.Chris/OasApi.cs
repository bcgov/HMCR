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
    public interface IOasApi
    {
        /// <summary>
        /// This query identifies if a point (specified by a longitude/latitude coordinate pair) is 
        /// within the specified distance (tolerance) of a specified RFI segment.  
        /// It requires BASIC authentication using an account with membership to TRAN ALL.
        /// </summary>
        /// <param name="tolerance">Metres of tolerance (i.e. 10)</param>
        /// <param name="point"></param>
        /// <param name="rfiSegment"></param>
        /// <returns>bool</returns>
        Task<bool> IsPointOnRfiSegmentAsync(int tolerance, Point point, string rfiSegment);
        /// <summary>
        /// This GET API call returns a geojson line feature 
        /// derived from the specified start and end offsets (0.02 km & 0.240 km) along a specified RFI segment (11-A-J-00949).
        /// </summary>
        /// <param name="rfiSegment"></param>
        /// <param name="start">Offset in KM</param>
        /// <param name="end">Offset in KM</param>
        /// <returns>Line</returns>
        Task<List<Line>> GetLineFromOffsetMeasureOnRfiSegmentAsync(string rfiSegment, decimal start, decimal end);
        /// <summary>
        /// This GET API call returns a geojson point feature that is snapped to the RFI and 
        /// includes a measure attribute derived from a specified longitude (-115.302974), latitude (49.375371), and
        /// a RFI segment ID (11-A-J-00949).  It requires BASIC authentication using an account with membership to TRAN ALL.
        /// Note: You should validate that the specified point is on (or within a given tolerance) to the RFI segment before running this function.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="rfiSegment"></param>
        /// <returns>success, offset, variance in KM</returns>   
        Task<(bool success, LrsPointResult result)> GetOffsetMeasureFromPointAndRfiSegmentAsync(Point point, string rfiSegment);
        /// <summary>
        /// This GET API call returns a geojson point feature derived 
        /// from the specified offset (0.02 km) along a specified RFI segment (11-A-J-00949).  
        /// It requires BASIC authentication using an account with membership to TRAN ALL.
        /// </summary>
        /// <param name="rfiSegment"></param>
        /// <param name="offset">Offset in KM<</param>
        /// <returns>LrsPointResult</returns>
        Task<Point> GetPointFromOffsetMeasureOnRfiSegmentAsync(string rfiSegment, decimal offset);
        /// <summary>
        /// This query retrieves a single RFI (Geometry and Attributes) in json format.  It should be used for validation purposes 
        /// such as ensuring that specified measures actually fall on an RFI.  
        /// It requires BASIC authentication using an account with membership to TRAN ALL.
        /// </summary>
        /// <param name="rfiSegment">Returns length in KM</param>
        /// <returns>RecordDimension (Point, Line, Na)</returns>
        Task<RfiSegment> GetRfiSegmentDetailAsync(string rfiSegment);
    }
    public class OasApi : IOasApi
    {
        private HttpClient _client;
        private OasQueries _queries;
        private IApi _api;
        private string _path;
        private ILogger<OasApi> _logger;

        public OasApi(HttpClient client, IApi api, IConfiguration config, ILogger<OasApi> logger)
        {
            _client = client;
            _api = api;
            _queries = new OasQueries();
            _path = config.GetValue<string>("CHRIS:OASPath");
            _logger = logger;
        }

        public async Task<bool> IsPointOnRfiSegmentAsync(int tolerance, Point point, string rfiSegment)
        {
            var body = "";
            var content = "";

            try
            {
                body = string.Format(_queries.PointOnRfiSegQuery, tolerance, point.Longitude, point.Latitude, rfiSegment);

                content = await (await _api.PostWithRetry(_client, _path, body)).Content.ReadAsStringAsync();

                var features = JsonSerializer.Deserialize<FeatureCollection<decimal[]>>(content);

                return features.numberMatched > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - IsPointOnRfiSegmentAsync: {body} - {content}");
                throw ex;
            }
        }

        public async Task<List<Line>> GetLineFromOffsetMeasureOnRfiSegmentAsync(string rfiSegment, decimal start, decimal end)
        {
            var query = "";
            var content = "";

            try
            {
                query = _path + string.Format(_queries.LineFromOffsetMeasureOnRfiSeg, rfiSegment, start, end);

                content = await (await _api.GetWithRetry(_client, query)).Content.ReadAsStringAsync();

                var simpleFeatures = JsonSerializer.Deserialize<FeatureCollection>(content);

                if (simpleFeatures.totalFeatures == 0) return null;

                var lines = new List<Line>();

                if (simpleFeatures.features[0].geometry.type.ToLowerInvariant() == "multilinestring")
                {
                    var multiline = JsonSerializer.Deserialize<FeatureCollection<decimal[][][]>>(content);

                    foreach (var line in multiline.features[0].geometry.coordinates)
                    {
                        lines.Add(new Line(line));
                    }
                }
                else
                {
                    var singleline = JsonSerializer.Deserialize<FeatureCollection<decimal[][]>>(content);
                    lines.Add(new Line(singleline.features[0].geometry.coordinates));
                }

                return lines;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - GetLineFromOffsetMeasureOnRfiSegmentAsync: {query} - {content}");
                throw ex;
            }
        }

        public async Task<(bool success, LrsPointResult result)> GetOffsetMeasureFromPointAndRfiSegmentAsync(Point point, string rfiSegment)
        {
            var query = "";
            var content = "";

            try
            {
                query = _path + string.Format(_queries.OffsetMeasureFromPointAndRfiSeg, point.Longitude, point.Latitude, rfiSegment);

                content = await (await _api.GetWithRetry(_client, query)).Content.ReadAsStringAsync();

                var features = JsonSerializer.Deserialize<FeatureCollection<decimal[]>>(content);

                if (features.totalFeatures == 0) return (false, null);

                return (true, new LrsPointResult(
                    Convert.ToDecimal(features.features[0].properties.MEASURE),
                    Convert.ToDecimal(features.features[0].properties.POINT_VARIANCE),
                    new Point(features.features[0].geometry.coordinates)));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - GetOffsetMeasureFromPointAndRfiSegmentAsync: {query} - {content}");
                throw ex;
            }
        }

        public async Task<Point> GetPointFromOffsetMeasureOnRfiSegmentAsync(string rfiSegment, decimal offset)
        {
            var query = "";
            var content = "";

            try
            {
                query = _path + string.Format(_queries.PointFromOffsetMeasureOnRfiSeg, rfiSegment, offset);

                content = await (await _api.GetWithRetry(_client, query)).Content.ReadAsStringAsync();

                var features = JsonSerializer.Deserialize<FeatureCollection<decimal[]>>(content);

                if (features.totalFeatures == 0) return null;

                return new Point(features.features[0].geometry.coordinates);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - GetPointFromOffsetMeasureOnRfiSegmentAsync: {query} - {content}");
                throw ex;
            }
        }

        public async Task<RfiSegment> GetRfiSegmentDetailAsync(string rfiSegment)
        {
            var query = "";
            var content = "";

            try
            {
                query = _path + string.Format(_queries.RfiSegmentDetail, rfiSegment);

                content = await (await _api.GetWithRetry(_client, query)).Content.ReadAsStringAsync();

                var features = JsonSerializer.Deserialize<FeatureCollection<object>>(content);

                if (features.totalFeatures == 0)
                    return new RfiSegment { Dimension = RecordDimension.Na };

                var feature = features.features[0];

                var dimension = feature.geometry.type.ToLower() == "point" ? RecordDimension.Point : RecordDimension.Line;

                return new RfiSegment { Dimension = dimension, Length = Convert.ToDecimal(feature.properties.NE_LENGTH), Descr = feature.properties.NE_DESCR };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - GetRfiSegmentDetailAsync: {query} - {content}");
                throw ex;
            }
        }
    }
}
