using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.LocationCode
{
    public class ActivityCodeDeleteDto
    {
        [JsonPropertyName("id")]
        public decimal ActivityCodeId { get; set; }
        public string ActivityNumber { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
