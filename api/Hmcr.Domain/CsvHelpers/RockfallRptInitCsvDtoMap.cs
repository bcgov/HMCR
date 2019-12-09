using CsvHelper.Configuration;
using Hmcr.Model.Dtos.RockfallReport;

namespace Hmcr.Domain.CsvHelpers
{
    public class RockfallRptInitCsvDtoMap : ClassMap<RockfallRptInitCsvDto>
    {
        public RockfallRptInitCsvDtoMap()
        {
            AutoMap();
            Map(m => m.ReportDate).TypeConverter<DateTypeConverter>();
        }
    }
}
