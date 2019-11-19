using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ServiceArea
{
    public class ServiceAreaNumberDto
    {
        [JsonPropertyName("id")]
        public decimal ServiceAreaNumber { get; set; }

        [JsonPropertyName("name")]
        public string ServiceAreaName { get; set; }
    }
}
