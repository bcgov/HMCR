using NetTopologySuite.Geometries;

namespace Hmcr.Model.Dtos.WildlifeReport
{
    public class WildlifeReportGeometry
    {
        public WildlifeReportTyped WildlifeReportTyped { get; set; }
        public Geometry Geometry { get; set; }

        public WildlifeReportGeometry(WildlifeReportTyped wildlifeReport, Geometry geometry)
        {
            WildlifeReportTyped = wildlifeReport;
            Geometry = geometry;
        }
    }
}
