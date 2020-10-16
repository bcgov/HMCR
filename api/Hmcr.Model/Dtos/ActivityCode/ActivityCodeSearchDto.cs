using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ActivityCode
{
    public class ActivityCodeSearchDto
    {
        [JsonPropertyName("id")]
        public decimal ActivityCodeId { get; set; }
        public string ActivityNumber { get; set; }
        public string ActivityName { get; set; }
        public string UnitOfMeasure { get; set; }
        public string MaintenanceType { get; set; }
        public decimal LocationCodeId { get; set; }
        public string FeatureType { get; set; }
        public string SpThresholdLevel { get; set; }
        public string ActivityApplication { get; set; }
        public bool IsSiteNumRequired { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive => EndDate == null || EndDate > DateTime.Today;
        public bool IsReferenced { get; set; }

        public decimal RoadLengthRule { get; set; }
        public decimal SurfaceTypeRule { get; set; }
        public decimal RoadClassRule { get; set; }
        public virtual IList<decimal> ServiceAreaNumbers { get; set; }

        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? ReportingFrequency { get; set; }
    }
}
