using Hmcr.Model.Dtos.SubmissionObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.SubmissionStatus
{
    public class SubmissionStatusDto
    {
        [JsonPropertyName("id")]
        public decimal StatusId { get; set; }
        public string StatusCode { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public string StatusType { get; set; }
    }
}
