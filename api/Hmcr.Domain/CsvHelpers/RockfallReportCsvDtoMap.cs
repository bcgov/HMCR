using CsvHelper.Configuration;
using Hmcr.Model;
using Hmcr.Model.Dtos.RockfallReport;
using System.Globalization;

namespace Hmcr.Domain.CsvHelpers
{
    public class RockfallRptInitCsvDtoMap : ClassMap<RockfallRptInitCsvDto>
    {
        public RockfallRptInitCsvDtoMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.ReportDate).TypeConverter<DateTypeConverter>();
        }
    }

    public class RockfallReportCsvDtoMap : ClassMap<RockfallReportCsvDto>
    {
        public RockfallReportCsvDtoMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
        }
    }

    public class RockfallReportDtoMap : ClassMap<RockfallReportTyped>
    {
        public RockfallReportDtoMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.ReportDate).TypeConverter<DateTypeConverter>();
            Map(m => m.EstimatedRockfallDate).TypeConverter<DateTypeConverter>();
            Map(m => m.McPhoneNumber).TypeConverter<PhoneNumberConverter>();
            Map(m => m.StartLatitude).TypeConverter<GpsCoordsConverter>();
            Map(m => m.EndLatitude).TypeConverter<GpsCoordsConverter>();
            Map(m => m.StartLongitude).TypeConverter<GpsCoordsConverter>();
            Map(m => m.EndLongitude).TypeConverter<GpsCoordsConverter>();
        }
    }
}
