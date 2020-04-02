using System;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos
{
    public class SpThresholdLevel
    {
        public int Error { get; set; }
        public int Warning { get; set; }

        [JsonPropertyName("id")]
        public string Level { get; set; }

        [JsonPropertyName("name")]
        public string Description
        {
            get
            {
                return $"{Level} (Warning: {Warning}m, {Error}m)";
            }
        }

        public SpThresholdLevel(string level, string codeText)
        {
            if (codeText == null)
            {
                return;
            }

            var csv = codeText.Split(',');

            if (csv.Length != 2)
            {
                throw new Exception($"Cannot parse THRSHLD_SP_VAR value {codeText}");
            }

            if (!int.TryParse(csv[0].Trim(), out int warning))
            {
                throw new Exception($"Cannot parse the first value of THRSHLD_SP_VAR value [{csv[0]}]");
            }
            else
            {
                Warning = warning;
            }

            if (!int.TryParse(csv[1].Trim(), out int error))
            {
                throw new Exception($"Cannot parse the second value of THRSHLD_SP_VAR value [{csv[1]}]");
            }
            else
            {
                Error = error;
            }

            Level = level;
        }
    }
}
