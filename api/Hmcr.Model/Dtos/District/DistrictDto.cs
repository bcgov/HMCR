using Hmcr.Model.Dtos.Region;
using Hmcr.Model.Dtos.ServiceArea;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.District
{
    public class DistrictDto
    {
        public decimal DistrictId { get; set; }
        [JsonPropertyName("id")]
        public decimal DistrictNumber { get; set; }
        [JsonPropertyName("name")]
        public string DistrictName { get; set; }
        public decimal RegionNumber { get; set; }
    }
}
