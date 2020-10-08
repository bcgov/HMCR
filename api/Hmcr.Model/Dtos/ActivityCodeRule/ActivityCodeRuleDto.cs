using System;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ActivityRule
{
    public class ActivityCodeRuleDto
    {
        [JsonPropertyName("id")]
        public decimal ActivityCodeRuleId { get; set; }
        [JsonPropertyName("name")]
        public string ActivityRuleName { get; set; }
        public string ActivityRuleSet { get; set; }
        public string ActivityRuleExecName { get; set; }
        public decimal DisplayOrder { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
