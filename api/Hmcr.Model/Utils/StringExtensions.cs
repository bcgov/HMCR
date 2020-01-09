using Hmcr.Model.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

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
        public static string GetErrorDetail(this Dictionary<string, List<string>> errors)
        {
            var errorDetail = new StringBuilder();
            foreach (var (error, detail) in errors.SelectMany(error => error.Value.Select(detail => (error, detail + Environment.NewLine))))
            {
                errorDetail.AppendLine($"{error.Key}: {detail}");
            }

            return errorDetail.ToString();
        }

        public static bool IsIdirUser(this string str)
        {
            return str.ToUpperInvariant() == UserTypeDto.INTERNAL;
        }

        public static bool IsBusinessUser(this string str)
        {
            return str.ToUpperInvariant() == UserTypeDto.BUSINESS;
        }

        public static string GetSha256Hash(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            using var sha = new System.Security.Cryptography.SHA256Managed();

            byte[] textData = Encoding.UTF8.GetBytes(text);
            byte[] hash = sha.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        public static string SanitizeFileName(this string fileName)
        {
            var invalids = Path.GetInvalidFileNameChars();
            var newFileName = String.Join("_", fileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            newFileName = Path.GetFileNameWithoutExtension(newFileName);

            if (newFileName.Length > 100)
            {
                newFileName = newFileName.Substring(0, 100);
            }

            return newFileName;
        }

        public static bool IsCsvFile(this string fileName)
        {
            if (!Path.HasExtension(fileName))
                return false;

            return Path.GetExtension(fileName).ToLowerInvariant() == ".csv";
        }
    }
}
