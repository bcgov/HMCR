using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Domain
{
    public static class RegexExp
    {
        public const string Email = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        public const string QREA = @"^[QREA]$";
        public const string D5_2 = @"^\-?\d{1,5}(\.\d{1,2})?$";
        public const string D5_6 = @"^\-?\d{1,5}(\.\d{1,6})?$";
        public const string D4_3 = @"^\-?\d{1,4}(\.\d{1,3})?$";
        public const string Dollar6_2 = @"^\$?\d{1,6}(\.\d{1,2})?$";
        public const string SiteNumber = @"^[ABDLRSTWX]\d{6}$";
    }
}
