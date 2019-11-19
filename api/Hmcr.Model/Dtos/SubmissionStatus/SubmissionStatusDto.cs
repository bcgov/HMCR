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
        public decimal SubmissionStatusId { get; set; }
        public string SubmissionStatusCode { get; set; }
        public string Description { get; set; }
    }
}
