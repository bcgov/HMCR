using CsvHelper;
using CsvHelper.Configuration;
using Hmcr.Model.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Hmcr.Domain.CsvHelpers
{
    public class CsvHelperUtils
    {
        public static void Config(Dictionary<string, List<string>> errors, CsvReader csv, bool checkHeader = true)
        {
            csv.Configuration.PrepareHeaderForMatch = (string header, int index) => Regex.Replace(header.ToLower(), @"[\s|\/]", string.Empty);
            csv.Configuration.CultureInfo = CultureInfo.GetCultureInfo("en-CA");

            csv.Configuration.TrimOptions = TrimOptions.Trim;

            if (checkHeader)
            {
                csv.Configuration.HeaderValidated = (bool valid, string[] column, int row, ReadingContext context) =>
                {
                    if (valid) return;

                    errors.AddItem($"{column[0]}", $"The header [{column[0].WordToWords()}] is missing.");
                };
            }
            else
            {
                csv.Configuration.MissingFieldFound = null;
                csv.Configuration.HeaderValidated = null;
            }
        }
    }
}
