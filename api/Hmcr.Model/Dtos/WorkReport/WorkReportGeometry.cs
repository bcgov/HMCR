using NetTopologySuite.Geometries;

namespace Hmcr.Model.Dtos.WorkReport
{
    public class WorkReportGeometry
    {
        public WorkReportTyped WorkReportTyped { get; set; }
        public Geometry Geometry { get; set; }
        public bool IsNonSpatial { get; set; }

        public WorkReportGeometry(WorkReportTyped workReport, Geometry geometry)
        {
            WorkReportTyped = workReport;
            Geometry = geometry;
            IsNonSpatial = false;
        }
    }
}
