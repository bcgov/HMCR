using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Hmcr.Model.Utils;
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
            if (date.IsEmpty())
                return null;

            if (date.Length > 8)
                date = date.Replace("-", "").Replace(".", "").Replace("/", "");

            return DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            throw new NotImplementedException();
        }
    }
}
