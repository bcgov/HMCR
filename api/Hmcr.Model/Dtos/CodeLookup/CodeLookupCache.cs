using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.CodeLookup
{
    public class CodeLookupCache
    {
        public string CodeSet { get; set; }

        /// <summary>
        /// This property has merged value of CodeValueText and CodeValueNum 
        /// CodeValue = x.CodeValueFormat == "NUMBER" ? x.CodeValueNum.ToString() : x.CodeValueText
        /// </summary>
        [JsonPropertyName("id")]
        public string CodeValue { get; set; }
        [JsonPropertyName("name")]
        public string CodeName { get; set; }
        public decimal? CodeValueNum { get; set; }
    }
}
