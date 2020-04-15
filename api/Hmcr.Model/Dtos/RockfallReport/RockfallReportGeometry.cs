using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.RockfallReport
{
    public class RockfallReportGeometry
    {
        public RockfallReportTyped RockfallReportTyped { get; set; }
        public Geometry Geometry { get; set; }

        public RockfallReportGeometry(RockfallReportTyped rockfallReport, Geometry geometry)
        {
            RockfallReportTyped = rockfallReport;
            Geometry = geometry;
        }
    }
}
