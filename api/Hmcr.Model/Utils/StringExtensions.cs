using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hmcr.Model.Utils
{
    public static class StringExtensions
    {
        public static bool IsNotEmpty(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static decimal[] ToDecimalArray(this string str)
        {
            if (str == null) return new decimal[] { };

            decimal[] result;

            try
            {
                string[] tokens = str.Split(',');

                result = new decimal[tokens.Length];

                for (int i = 0; i < tokens.Length; i++)
                {
                    result[i] = decimal.Parse(tokens[i]);
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public static string[] ToStringArray(this string str)
        {
            return str == null ? (new string[] { }) : str.Split(',');
        }

        public static string WordToWords(this string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", x => $"{x.Value[0]} {char.ToUpper(x.Value[1])}");
        }

        public static string RemoveLineBreak(this string str)
        {
            return Regex.Replace(str, @"\r\n?|\n", "");
        }

        public static bool IsInteger(this string str)
        {
            return Regex.IsMatch(str, @"^\d+$");
        }

        public static void AddItem(this Dictionary<string, List<string>> dictionary, string keyName, string item)
        {
            if (dictionary.ContainsKey(keyName))
            {
                dictionary[keyName].Add(item);
            }
            else
            {
                dictionary.Add(keyName, new List<string> { item });
            }
        }
    }
}
