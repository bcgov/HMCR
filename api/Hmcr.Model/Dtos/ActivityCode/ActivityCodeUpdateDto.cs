using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ActivityCode
{
    public class ActivityCodeUpdateDto
    {
        [JsonPropertyName("id")]
        public decimal ActivityCodeId { get; set; }
        public string ActivityName { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
