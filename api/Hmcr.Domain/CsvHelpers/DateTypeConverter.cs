using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Hmcr.Model.Utils;
using System;

namespace Hmcr.Domain.CsvHelpers
{
    public class DateTypeConverter : ITypeConverter
    {
        public object ConvertFromString(string date, IReaderRow row, MemberMapData memberMapData)
        {
            var (parsed, parsedDate) = DateUtils.ParseDate(date);

            if (!parsed)
                throw new Exception($"The value [{date}] cannot be parsed into date.");

            return parsedDate;
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            throw new NotImplementedException();
        }
    }
}
