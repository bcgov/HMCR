using CsvHelper.Configuration;
using Hmcr.Model.Dtos.WorkReport;

namespace Hmcr.Domain.CsvHelpers
{
    public class WorkRptInitCsvDtoMap : ClassMap<WorkRptInitCsvDto>
    {
        public WorkRptInitCsvDtoMap()
        {
            AutoMap();
            Map(m => m.EndDate)
                .TypeConverter<DateTypeConverter>();
        }
    }

    public class WorkReportCsvDtoMap : ClassMap<WorkReportCsvDto>
    {
        public WorkReportCsvDtoMap()
        {
            AutoMap();
            Map(m => m.ValueOfWork).TypeConverter<DollarValueConverter>();
        }
    }

    public class WorkReportDtoMap : ClassMap<WorkReportDto>
    {
        public WorkReportDtoMap()
        {
            AutoMap();
            Map(m => m.StartDate).TypeConverter<DateTypeConverter>();
            Map(m => m.EndDate).TypeConverter<DateTypeConverter>();
            Map(m => m.PostedDate).TypeConverter<DateTypeConverter>();
        }
    }
}
