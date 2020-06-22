using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Hmcr.Model.Utils;
using System;

namespace Hmcr.Domain.CsvHelpers
{
    public class GpsCoordsToStringConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text.IsEmpty())
                return null;

            var value = text.Trim();

            return value == "0" ? null : value;
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            throw new NotImplementedException();
        }
    }
}
