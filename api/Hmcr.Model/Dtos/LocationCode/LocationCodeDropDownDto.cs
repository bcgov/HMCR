using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.LocationCode
{
    public class LocationCodeDropDownDto
    {
        [JsonPropertyName("id")]
        public decimal LocationCodeId { get; set; }
        [JsonPropertyName("name")]
        public string LocationCode { get; set; }
    }
}
