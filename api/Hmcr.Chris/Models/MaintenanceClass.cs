using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Chris.Models
{
    public class MaintenanceClass
    {

        public Geometry geometry { get; set; }
        /// <summary>
        /// Length in Meters
        /// </summary>
        public string SummerRating { get; set; }
        public string WinterRating { get; set; }
        public double Length { get; set; }
    }
}
