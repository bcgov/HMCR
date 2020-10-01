using System;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ActivityRule
{
    public class ActivityRuleDto
    {
        [JsonPropertyName("id")]
        public decimal ActivityRuleId { get; set; }
        [JsonPropertyName("name")]
        public string ActivityRuleName { get; set; }
        public string ActivityRuleSet { get; set; }
        public string ActivityRuleExecName { get; set; }
        public decimal DisplayOrder { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
