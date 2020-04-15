using System;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ActivityCode
{
    public class ActivityCodeUpdateDto
    {
        [JsonPropertyName("id")]
        public decimal ActivityCodeId { get; set; }
        public string ActivityName { get; set; }
        public decimal LocationCodeId { get; set; }
        public string FeatureType { get; set; }
        public string SpThresholdLevel { get; set; }
        public bool IsSiteNumRequired { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
