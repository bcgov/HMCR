using System;
using System.Globalization;

namespace Hmcr.Model.Utils
{
    public static class DateUtils
    {
        public static (bool parsed, DateTime? parsedDate) ParseDate(object val)
        {
            if (val == null)
                return (true, null);

            if (val.GetType() == typeof(DateTime))
            {
                return (true, (DateTime)val);
            }

            var formats = new string[] { "yyyyMMdd", "yyyy-MM-dd", "yyyy/MM/dd", "yyyy.MM.dd" };
            var dateStr = val.ToString();

            if (dateStr.IsEmpty())
                return (true, null);

            return (DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate), parsedDate);
        }

        public static string CovertToString(DateTime date)
        {
            return date == null ? "" : date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Returns Pacific time if VancouverTimeZone or PacificTimeZone is defined in the system
        /// Otherwise reutnrs UTC time.
        /// </summary>
        /// <param name="utcDate"></param>
        /// <returns></returns>
        public static DateTime ConvertUtcToPacificTime(DateTime utcDate)
        {
            var date = GetLocalTime(utcDate, Constants.VancouverTimeZone);

            if (date != null)
                return (DateTime)date;

            date = GetLocalTime(utcDate, Constants.PacificTimeZone);

            if (date != null)
                return (DateTime)date;

            return utcDate;
        }

        private static DateTime? GetLocalTime(DateTime utcDate, string id)
        {
            try
            {
                var timezone = TimeZoneInfo.FindSystemTimeZoneById(id);
                return TimeZoneInfo.ConvertTimeFromUtc(utcDate, timezone);
            }
            catch (TimeZoneNotFoundException)
            {
                return null;
            }
        }
    }
}
