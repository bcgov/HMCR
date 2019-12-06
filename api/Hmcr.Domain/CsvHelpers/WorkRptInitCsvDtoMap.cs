using CsvHelper.Configuration;
using Hmcr.Model.Dtos.WorkReport;

namespace Hmcr.Domain.CsvHelpers
{
    public class WorkRptInitCsvDtoMap : ClassMap<WorkRptInitCsvDto>
    {
        public WorkRptInitCsvDtoMap()
        {
            AutoMap();
            Map(m => m.EndDate).TypeConverter<DateTypeConverter>();
        }
    }
}
