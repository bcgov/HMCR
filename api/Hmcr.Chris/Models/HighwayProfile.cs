using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Chris.Models
{
    public class HighwayProfile
    {
        public Geometry geometry { get; set; }
        /// <summary>
        /// Length in Meters
        /// </summary>
        public int NumberOfLanes { get; set; }
        public string DividedHighwayFlag { get; set; }
        public double Length { get; set; }
    }
}
