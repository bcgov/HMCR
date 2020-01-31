using CsvHelper.Configuration;
using Hmcr.Model.Dtos.WildlifeReport;

namespace Hmcr.Domain.CsvHelpers
{
    class WildlifeRptInitCsvDtoMap : ClassMap<WildlifeRptInitCsvDto>
    {
        public WildlifeRptInitCsvDtoMap()
        {
            AutoMap();
        }
    }
    public class WildlifeReportCsvDtoMap : ClassMap<WildlifeReportCsvDto>
    {
        public WildlifeReportCsvDtoMap()
        {
            AutoMap();
        }
    }

    public class WildlifeReportDtoMap : ClassMap<WildlifeReportDto>
    {
        public WildlifeReportDtoMap()
        {
            AutoMap();
            Map(m => m.AccidentDate).TypeConverter<DateTypeConverter>();
        }
    }
}