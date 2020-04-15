using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Text.RegularExpressions;

namespace Hmcr.Domain.CsvHelpers
{
    public class PhoneNumberConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return Regex.Replace(text, @"[\s|\/|\.|-]", string.Empty);
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            throw new NotImplementedException();
        }
    }
}
