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
            Map(m => m.StartLatitude).TypeConverter<GpsCoordsToStringConverter>();
            Map(m => m.EndLatitude).TypeConverter<GpsCoordsToStringConverter>();
            Map(m => m.StartLongitude).TypeConverter<GpsCoordsToStringConverter>();
            Map(m => m.EndLongitude).TypeConverter<GpsCoordsToStringConverter>();
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
            Map(m => m.StartLatitude).TypeConverter<GpsCoordsToNumberConverter>();
            Map(m => m.EndLatitude).TypeConverter<GpsCoordsToNumberConverter>();
            Map(m => m.StartLongitude).TypeConverter<GpsCoordsToNumberConverter>();
            Map(m => m.EndLongitude).TypeConverter<GpsCoordsToNumberConverter>();
        }
    }
}
