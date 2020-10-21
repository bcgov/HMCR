using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.WorkReport
{
    public class WorkReportRoadFeature
    {
        public string SurfaceType { get; set; }
        public double SurfaceLength { get; set; }

        public WorkReportRoadFeature(string surfaceType, double surfaceLength)
        {
            SurfaceType = surfaceType;
            SurfaceLength = surfaceLength;
        }
    }
}