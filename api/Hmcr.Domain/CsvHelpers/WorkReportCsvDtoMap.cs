using CsvHelper.Configuration;
using Hmcr.Model.Dtos.WorkReport;
using System.Globalization;

namespace Hmcr.Domain.CsvHelpers
{
    public class WorkRptInitCsvDtoMap : ClassMap<WorkRptInitCsvDto>
    {
        public WorkRptInitCsvDtoMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.EndDate).TypeConverter<DateTypeConverter>();
        }
    }

    public class WorkReportCsvDtoMap : ClassMap<WorkReportCsvDto>
    {
        public WorkReportCsvDtoMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.ValueOfWork).TypeConverter<DollarValueToStringConverter>();
            Map(m => m.Accomplishment).TypeConverter<DollarValueToStringConverter>();
        }
    }

    public class WorkReportDtoMap : ClassMap<WorkReportTyped>
    {
        public WorkReportDtoMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.StartDate).TypeConverter<DateTypeConverter>();
            Map(m => m.EndDate).TypeConverter<DateTypeConverter>();
            Map(m => m.PostedDate).TypeConverter<DateTypeConverter>();
            Map(m => m.ValueOfWork).TypeConverter<DollarValueToNumberConverter>();
            Map(m => m.Accomplishment).TypeConverter<DollarValueToNumberConverter>();
        }
    }
}
