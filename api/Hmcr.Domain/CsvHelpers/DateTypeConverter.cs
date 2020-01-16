using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Hmcr.Model.Utils;

namespace Hmcr.Domain.CsvHelpers
{
    public class DateTypeConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string date, IReaderRow row, MemberMapData memberMapData)
        {
            var (parsed, parsedDate) = DateUtils.ParseDate(date);

            if (!parsed)
                throw new TypeConverterException(this, memberMapData, date, (ReadingContext)row.Context, $"The value [{date}] cannot be parsed into date.");

            return parsedDate;
        }
    }
}