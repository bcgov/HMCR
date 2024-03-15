using System;
using System.Collections.Generic;
using System.Linq;

namespace Hmcr.Data.Database.Entities
{
    public static class HmrSaltReportExtensions
    {
        public static int GetTotalYesResponses(this IEnumerable<HmrSaltReport> reports, Func<HmrSaltReport, string> selector)
        {
            return reports.Count(report => string.Equals(selector(report), "Yes", StringComparison.OrdinalIgnoreCase));
        }

        public static int GetTotalIntValue(this IEnumerable<HmrSaltReport> reports, Func<HmrSaltReport, int?> selector)
        {
            return reports.Sum(report => selector(report) ?? 0);
        }

        public static decimal GetTotalDecimalValue(this IEnumerable<HmrSaltReport> reports, Func<HmrSaltReport, decimal?> selector)
        {
            return reports.Sum(report => selector(report) ?? 0);
        }

        public static int GetTotalBoolValue(this IEnumerable<HmrSaltReport> reports, Func<HmrSaltReport, bool?> selector)
        {
            return reports.Count(report => selector(report) == true);
        }
    }
}