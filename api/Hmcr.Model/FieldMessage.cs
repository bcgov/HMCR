using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model
{
    public class FieldMessage
    {
        [JsonPropertyName("field")]
        public string Field { get; set; }
        [JsonPropertyName("messages")]
        public List<string> Messages { get; set; }
    }
}
