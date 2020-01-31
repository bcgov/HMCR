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
    }
}
