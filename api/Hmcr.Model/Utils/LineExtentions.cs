using Hmcr.Chris.Models;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Hmcr.Model.Utils
{
    public static class LineExtentions
    {
        public static Coordinate[] ToTopologyCoordinates(this Line line)
        {
            var coordinates = new List<Coordinate>();

            foreach(var point in line.Points)
            {
                coordinates.Add(point.ToTopologyCoordinate());
            }

            return coordinates.ToArray();
        }
    }
}
