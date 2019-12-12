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
            //Map(m => m.ServiceArea).Name("contractservicearea");
        }
    }
}