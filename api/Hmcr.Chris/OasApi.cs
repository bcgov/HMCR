using Hmcr.Chris.Models;
using System;
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
        Task<Line> GetLineFromOffsetMeasuerOnRfiSegmentAsync(string rfiSegment, decimal start, decimal end);
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
        private const string _path = "ogs-geoV06/wfs?";

        public OasApi(HttpClient client, IApi api)
        {
            _client = client;
            _api = api;
            _queries = new OasQueries();
        }

        public async Task<bool> IsPointOnRfiSegmentAsync(int tolerance, Point point, string rfiSegment)
        {
            var body = string.Format(_queries.PointOnRfiSegQuery, tolerance, point.Longitude, point.Latitude, rfiSegment);

            var contents = await _api.Post(_client, _path, body);

            var features = JsonSerializer.Deserialize<FeatureCollection<decimal[]>>(contents);

            return features.numberMatched > 0;
        }

        public async Task<Line> GetLineFromOffsetMeasuerOnRfiSegmentAsync(string rfiSegment, decimal start, decimal end)
        {
            var query = _path + string.Format(_queries.LineFromOffsetMeasureOnRfiSeg, rfiSegment, start, end);

            var content = await _api.Get(_client, query);

            var features = JsonSerializer.Deserialize<FeatureCollection<decimal[][]>>(content);

            if (features.totalFeatures == 0) return null;

            return new Line(features.features[0].geometry.coordinates);
        }

        public async Task<(bool success, LrsPointResult result)> GetOffsetMeasureFromPointAndRfiSegmentAsync(Point point, string rfiSegment)
        {
            var query = _path + string.Format(_queries.OffsetMeasureFromPointAndRfiSeg, point.Longitude, point.Latitude, rfiSegment);

            var content = await _api.Get(_client, query);

            var features = JsonSerializer.Deserialize<FeatureCollection<decimal[]>>(content);

            if (features.totalFeatures == 0) return (false, null);

            return (true, new LrsPointResult(
                Convert.ToDecimal(features.features[0].properties.MEASURE),
                Convert.ToDecimal(features.features[0].properties.POINT_VARIANCE),
                new Point(features.features[0].geometry.coordinates)));
        }

        public async Task<Point> GetPointFromOffsetMeasureOnRfiSegmentAsync(string rfiSegment, decimal offset)
        {
            var query = _path + string.Format(_queries.PointFromOffsetMeasureOnRfiSeg, rfiSegment, offset);

            var content = await _api.Get(_client, query);

            var features = JsonSerializer.Deserialize<FeatureCollection<decimal[]>>(content);

            if (features.totalFeatures == 0) return null;

            return new Point(features.features[0].geometry.coordinates);
        }
        public async Task<RfiSegment> GetRfiSegmentDetailAsync(string rfiSegment)
        {
            var query = _path + string.Format(_queries.RfiSegmentDetail, rfiSegment);

            var content = await _api.Get(_client, query);

            var features = JsonSerializer.Deserialize<FeatureCollection<object>>(content);

            if (features.totalFeatures == 0)
                return new RfiSegment { Dimension = RecordDimension.Na };

            var feature = features.features[0];

            var dimension = feature.geometry.type.ToLower() == "point" ? RecordDimension.Point : RecordDimension.Line;

            return new RfiSegment { Dimension = dimension, Length = Convert.ToDecimal(feature.properties.NE_LENGTH), Descr = feature.properties.NE_DESCR };
        }
    }
}
