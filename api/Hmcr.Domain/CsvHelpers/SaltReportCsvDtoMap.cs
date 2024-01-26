using CsvHelper.Configuration;
using Hmcr.Data.Database.Entities;
using Hmcr.Model.Dtos.WildlifeReport;
using System.Globalization;

namespace Hmcr.Domain.CsvHelpers
{
    public sealed class SaltReportMap : ClassMap<HmrSaltReport>
    {
        public SaltReportMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.SaltReportId).Ignore();

            Map(m => m.Stockpiles).Ignore();

            Map(m => m.ConcurrencyControlNumber).Ignore();
            Map(m => m.AppCreateUserid).Ignore();
            Map(m => m.AppCreateTimestamp).Ignore();
            Map(m => m.AppCreateUserGuid).Ignore();
            Map(m => m.AppCreateUserDirectory).Ignore();
            Map(m => m.AppLastUpdateUserid).Ignore();
            Map(m => m.AppLastUpdateTimestamp).Ignore();
            Map(m => m.AppLastUpdateUserGuid).Ignore();
            Map(m => m.AppLastUpdateUserDirectory).Ignore();
            Map(m => m.DbAuditCreateUserid).Ignore();
            Map(m => m.DbAuditCreateTimestamp).Ignore();
            Map(m => m.DbAuditLastUpdateUserid).Ignore();
            Map(m => m.DbAuditLastUpdateTimestamp).Ignore();
            // Map other properties...
        }
    }
}