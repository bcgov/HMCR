using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.WorkReport
{
    public class WorkReportRoadFeature
    {
        public string SurfaceType { get; set; }
        public double SurfaceLength { get; set; }

        public string SummerRating { get; set; }
        public string WinterRating { get; set; }
        public double RoadLength { get; set; }
    }
}