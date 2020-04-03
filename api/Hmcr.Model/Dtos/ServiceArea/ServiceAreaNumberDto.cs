using Hmcr.Model.Utils;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ServiceArea
{
    public class ServiceAreaNumberDto
    {
        [JsonPropertyName("id")]
        public decimal ServiceAreaNumber { get; set; }
        public string ServiceAreaName { get; set; }
        public string Name => $"{ServiceAreaNumber} {ServiceAreaName}";

        public string HighwayUniquePrefix { get; set; }
        private string[] _prefixes = null;
        public string[] HighwayUniquePrefixes => _prefixes ??= ParsePrefix();
        private string[] ParsePrefix()
        {
            var prefixes = new List<string>();

            if (HighwayUniquePrefix.IsEmpty())
            {
                prefixes.Add(string.Format("{0, 2:00}", ServiceAreaNumber));
                return prefixes.ToArray();
            }

            var csv = HighwayUniquePrefix.Split(',');

            if (csv.Length == 0)
            {
                prefixes.Add(string.Format("{0, 2:00}", ServiceAreaNumber));
                return prefixes.ToArray();
            }

            foreach (var prefix in csv)
            {
                prefixes.Add(prefix.Trim());
            }

            return prefixes.ToArray();
        }
    }
}
