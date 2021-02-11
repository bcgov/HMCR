using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Chris.Models
{
    public class SurfaceType
    {

        public Geometry geometry { get; set; }
        /// <summary>
        /// Length in Meters
        /// </summary>
        public double Length { get; set; }

        public string Type { get; set; }
        
    }
    public class SurfaceType<T>
    {

        public Geometry geometry { get; set; }
        /// <summary>
        /// Length in Meters
        /// </summary>
        public double Length { get; set; }

        public string Type { get; set; }

    }
}
