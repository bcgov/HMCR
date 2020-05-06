using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Hmcr.Model.Utils;
using System;

namespace Hmcr.Domain.CsvHelpers
{
    public class DollarValueToNumberConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            var value = text.Replace("$", "").Replace(",", "").Trim();

            if (value.IsEmpty())
            {
                return null;
            }

            return Convert.ToDecimal(value);
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            throw new NotImplementedException();
        }
    }
}