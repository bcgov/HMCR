using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Chris.Models
{
    public class Structure
    {
        public Geometry geometry { get; set; }
        /// <summary>
        /// Length in Meters
        /// </summary>
        public string StructureType { get; set; }
        public decimal BeginKM { get; set; }
        public decimal EndKM { get; set; }
        public double Length { get; set; }
    }
}
