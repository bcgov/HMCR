using Hmcr.Model.Dtos.LocationCode;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ActivityCode
{
    public class ActivityCodeValidationDto
    {
        public string LocationCode { get; set; }

        public decimal RoadLengthRuleId { get; set; }
        public string RoadLenghRuleExec { get; set; }
        public decimal SurfaceTypeRuleId { get; set; }
        public string SurfaceTypeRuleExec { get; set; }
        public decimal RoadClassRuleId { get; set; }
        public string RoadClassRuleExec { get; set; }
        public virtual IList<decimal> ServiceAreaNumbers { get; set; }

        public decimal? MinimumValue { get; set; }
        public decimal? MaximumValue { get; set; }
        public decimal? ReportingFrequency { get; set; }
    }
}
