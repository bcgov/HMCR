using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.ServiceAreaActivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ServiceAreaActivity
{
    public class ServiceAreaActivityDto
    {
        [JsonPropertyName("id")]
        public decimal ServiceAreaActivityId { get; set; }
        public decimal ActivityCodeId { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
