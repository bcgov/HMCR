using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.MimeType
{
    public class MimeTypeDto
    {
        [JsonPropertyName("id")]
        public decimal MimeTypeId { get; set; }
        public string MimeTypeCode { get; set; }
        public string Description { get; set; }
    }
}
