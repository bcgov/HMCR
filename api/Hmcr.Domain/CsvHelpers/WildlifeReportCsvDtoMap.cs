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
            Map(m => m.Latitude).TypeConverter<GpsCoordsToStringConverter>();
            Map(m => m.Longitude).TypeConverter<GpsCoordsToStringConverter>();
        }
    }

    public class WildlifeReportDtoMap : ClassMap<WildlifeReportTyped>
    {
        public WildlifeReportDtoMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.AccidentDate).TypeConverter<DateTypeConverter>();
            Map(m => m.Latitude).TypeConverter<GpsCoordsToNumberConverter>();
            Map(m => m.Longitude).TypeConverter<GpsCoordsToNumberConverter>();
        }
    }
}