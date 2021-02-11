using System;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ActivityRule
{
    public class ActivityCodeRuleCache
    {
        [JsonPropertyName("id")]
        public decimal ActivityCodeRuleId { get; set; }
        [JsonPropertyName("name")]
        public string ActivityRuleSet { get; set; }
        public string ActivityRuleName { get; set; }
        public string ActivityRuleExecName { get; set; }
        
    }
}