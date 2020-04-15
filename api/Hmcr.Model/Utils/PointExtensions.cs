using NetTopologySuite.Geometries;

namespace Hmcr.Model.Utils
{
    public static class PointExtensions
    {
        public static Coordinate ToTopologyCoordinate(this Hmcr.Chris.Models.Point point)
        {
            return new Coordinate((double)point.Longitude, (double)point.Latitude);
        }
    }
}
