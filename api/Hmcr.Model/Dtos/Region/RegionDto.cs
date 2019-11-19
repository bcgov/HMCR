using Hmcr.Model.Dtos.District;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.Region
{
    public class RegionDto
    {
        public decimal RegionId { get; set; }
        [JsonPropertyName("id")]
        public decimal RegionNumber { get; set; }
        [JsonPropertyName("name")]
        public string RegionName { get; set; }
    }
}
