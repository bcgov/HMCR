using System;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ActivityRule
{
    public class RoadLengthRuleDto
    {
        [JsonPropertyName("id")]
        public decimal RoadLengthRuleId { get; set; }
        [JsonPropertyName("name")]
        public string RoadLengthRuleName { get; set; }
        public string ActivityRuleSet { get; set; }
        public string ActivityRuleExecName { get; set; }
        public decimal DisplayOrder { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
