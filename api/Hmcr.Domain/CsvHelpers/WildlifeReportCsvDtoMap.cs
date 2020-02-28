using CsvHelper.Configuration;
using Hmcr.Model.Dtos.WildlifeReport;
using System.Globalization;

namespace Hmcr.Domain.CsvHelpers
{
    class WildlifeRptInitCsvDtoMap : ClassMap<WildlifeRptInitCsvDto>
    {
        public WildlifeRptInitCsvDtoMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.AccidentDate).TypeConverter<DateTypeConverter>();
        }
    }
    public class WildlifeReportCsvDtoMap : ClassMap<WildlifeReportCsvDto>
    {
        public WildlifeReportCsvDtoMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
        }
    }

    public class WildlifeReportDtoMap : ClassMap<WildlifeReportTyped>
    {
        public WildlifeReportDtoMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.AccidentDate).TypeConverter<DateTypeConverter>();
        }
    }
}