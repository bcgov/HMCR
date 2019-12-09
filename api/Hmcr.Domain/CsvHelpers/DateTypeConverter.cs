using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Hmcr.Domain.CsvHelpers
{
    public class DateTypeConverter : ITypeConverter
    {
        public object ConvertFromString(string date, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(date))
                return null;

            if (date.Length == 8)
                return DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            else 
                return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            throw new NotImplementedException();
        }
    }
}
