using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Chris.Models
{
    public class Guardrail
    {
        public Geometry geometry { get; set; }
        /// <summary>
        /// Length in Meters
        /// </summary>
        public string GuardrailType { get; set; }
        public double Length { get; set; }
        public string CrossSectionPosition { get; set; }
    }
}
