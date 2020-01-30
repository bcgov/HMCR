using CsvHelper.Configuration;
using Hmcr.Model.Dtos.RockfallReport;
using Hmcr.Model.Dtos.WildlifeReport;
using System;
using System.Collections.Generic;
using System.Text;

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
            Map(m => m.Latitude).TypeConverter<NullableDecimalConverter>();
            Map(m => m.Longitude).TypeConverter<NullableDecimalConverter>();
            Map(m => m.StartOffset).TypeConverter<NullableDecimalConverter>();
            Map(m => m.Quantity).TypeConverter<NullableDecimalConverter>();
            Map(m => m.Species).TypeConverter<NullableDecimalConverter>();
        }
    }
}