using Hmcr.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public string ServiceArea => ((long)ServiceAreaNumber).ToString();

        public string ConvertToServiceAreaString(string text)
        {
            if (text.IsEmpty())
                return null;

            if (!text.IsInteger())
                return text;

            var value = string.Format("{0, 2:00}", Convert.ToInt32(text));

            if (HighwayUniquePrefixes.Contains(value))
                return ServiceArea;

            return text;
        }

        public decimal ConvertToServiceAreaNumber(decimal number)
        {
            var value = string.Format("{0, 2:00}", Convert.ToInt32(number));

            if (HighwayUniquePrefixes.Contains(value))
                return ServiceAreaNumber;

            return number;
        }
    }
}
