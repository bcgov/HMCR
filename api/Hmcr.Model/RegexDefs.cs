using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model
{
    public class RegexInfo
    {
        public string Regex { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class RegexDefs
    {
        public const string Email = "Email";
        public const string QREA = "QREA";
        public const string D5_2 = "D5_2";
        public const string D5_6 = "D5_6";
        public const string D4_3 = "D4_3";
        public const string Dollar6_2 = "Dollar6_2";
        public const string SiteNumber = "SiteNumber";

        private Dictionary<string, RegexInfo> _regexInfos;

        public RegexDefs()
        {
            _regexInfos = new Dictionary<string, RegexInfo>();

            _regexInfos.Add(Email, new RegexInfo { Regex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Wrong email address" });
            _regexInfos.Add(QREA, new RegexInfo { Regex = @"^[QREA]$", ErrorMessage = "Value must be one of these [QREA] letters" });
            _regexInfos.Add(D5_2, new RegexInfo { Regex = @"^\-?\d{1,5}(\.\d{1,2})?$", ErrorMessage = "Value must be a number of less than 5 digits optionally with maximum 2 decimal digits" });
            _regexInfos.Add(D5_6, new RegexInfo { Regex = @"^\-?\d{1,5}(\.\d{1,6})?$", ErrorMessage = "Value must be a number of less than 5 digits optionally with maximum 6 decimal digits" });
            _regexInfos.Add(D4_3, new RegexInfo { Regex = @"^\-?\d{1,4}(\.\d{1,3})?$", ErrorMessage = "Value must be a number of less than 4 digits optionally with maximum 3 decimal digits" });
            _regexInfos.Add(Dollar6_2, new RegexInfo { Regex = @"^\$?\d{1,6}(\.\d{1,2})?$", ErrorMessage = "Value must be a number of less than 6 digits optionally with maximum 2 decimal digits" });
            _regexInfos.Add(SiteNumber, new RegexInfo { Regex = @"^[ABDLRSTWX]\d{6}$", ErrorMessage = "Value must start with one of these [ABDLRSTWX] letters followed by 6 digit number" });
        }

        public RegexInfo GetRegexInfo(string name)
        {
            if (!_regexInfos.TryGetValue(name, out RegexInfo regexInfo))
            {
                throw new Exception($"RegexInfo for {name} does not exist.");
            }

            return regexInfo;
        }
    }
}
