using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Hmcr.Model.Utils
{
    public class DateUtils
    {
        public static (bool parsed, DateTime? parsedDate) ParseDate(object val)
        {
            var parsedDate = new DateTime();

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

            return (DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate), parsedDate);
        }
    }
}
