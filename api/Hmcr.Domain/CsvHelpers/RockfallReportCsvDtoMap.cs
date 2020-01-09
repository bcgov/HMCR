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
            Map(m => m.MajorIncidentNumber).Name(Fields.McrrIncidentNumber.ToLower());
        }
    }

    public class RockfallReportCsvDtoMap : ClassMap<RockfallReportCsvDto>
    {
        public RockfallReportCsvDtoMap()
        {
            AutoMap();
            Map(m => m.MajorIncidentNumber).Name(Fields.McrrIncidentNumber.ToLower());
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
            Map(m => m.MajorIncidentNumber).Name(Fields.McrrIncidentNumber.ToLower());
            Map(m => m.ReporterName).Name(Fields.Name.ToLower());
        }
    }
}
