using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Chris.Models
{
    public class RestArea
    {
        public Geometry geometry { get; set; }
        /// <summary>
        /// Length in Meters
        /// </summary>
        public string SiteNumber { get; set; }
        public decimal LocationKM { get; set; }
        public string Class { get; set; }
    }
}
