using System;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.LocationCode
{
    public class ActivityCodeUpdateDto
    {
        [JsonPropertyName("id")]
        public decimal ActivityCodeId { get; set; }
        public string ActivityName { get; set; }
        public decimal LocationCodeId { get; set; }
        public string PointLineFeature { get; set; }
        public bool IsSiteNumRequired { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
