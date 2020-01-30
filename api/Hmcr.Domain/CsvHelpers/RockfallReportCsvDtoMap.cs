using CsvHelper.Configuration;
using Hmcr.Model;
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

    public class RockfallReportCsvDtoMap : ClassMap<RockfallReportCsvDto>
    {
        public RockfallReportCsvDtoMap()
        {
            AutoMap();
        }
    }

    public class RockfallReportDtoMap : ClassMap<RockfallReportDto>
    {
        public RockfallReportDtoMap()
        {
            AutoMap();
            Map(m => m.ReportDate).TypeConverter<DateTypeConverter>();
            Map(m => m.EstimatedRockfallDate).TypeConverter<DateTypeConverter>();
            Map(m => m.McPhoneNumber).TypeConverter<PhoneNumberConverter>();
            Map(m => m.ReporterName).Name(Fields.Name.ToLower());

            Map(m => m.StartLatitude).TypeConverter<NullableDecimalConverter>();
            Map(m => m.EndLatitude).TypeConverter<NullableDecimalConverter>();
            Map(m => m.StartLongitude).TypeConverter<NullableDecimalConverter>();
            Map(m => m.EndLongitude).TypeConverter<NullableDecimalConverter>();
            Map(m => m.StartOffset).TypeConverter<NullableDecimalConverter>();
            Map(m => m.EndOffset).TypeConverter<NullableDecimalConverter>();

            Map(m => m.OtherVolume).TypeConverter<NullableDecimalConverter>();
        }
    }
}
