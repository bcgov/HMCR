using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.CodeLookup
{
    public class CodeLookupForValidation
    {
        public string CodeSet { get; set; }
        [JsonPropertyName("id")]
        public string CodeValue { get; set; }
        [JsonPropertyName("desc")]
        public string CodeName { get; set; }
    }
}
