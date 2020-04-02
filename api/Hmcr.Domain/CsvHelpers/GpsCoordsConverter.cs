using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Hmcr.Model.Utils;
using System;

namespace Hmcr.Domain.CsvHelpers
{
    public class GpsCoordsConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text.IsEmpty())
                return null;

            var value = Convert.ToDecimal(text);

            return value == 0 ? null : (object)value;
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            throw new NotImplementedException();
        }
    }
}
