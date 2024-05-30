using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Linq;

namespace Hmcr.Domain.CsvHelpers
{
    public class BooleanYesNoConverter : ITypeConverter
    {
        private static readonly string[] TrueStrings = { "yes", "y", "true", "t", "1" };
        private static readonly string[] FalseStrings = { "no", "n", "false", "f", "0" };

        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            text = text.Trim();

            if (TrueStrings.Contains(text, StringComparer.OrdinalIgnoreCase))
                return true;

            if (FalseStrings.Contains(text, StringComparer.OrdinalIgnoreCase))
                return false;

            throw new ArgumentException($"Invalid boolean value: {text}");
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null)
            {
                return ""; // or "No" or any other representation for null values
            }

            return (bool)value ? "Yes" : "No";
        }
    }

}
