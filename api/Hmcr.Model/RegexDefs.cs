﻿using System;
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
        public const string GpsCoords = "D14_9"; //latitude longitude
        public const string Offset = "D7_3"; //offset
        public const string DollarValue = "D9_2"; //value of work, accomplishment
        public const string Volume = "D6_2"; //rockfall volume
        public const string Quantity = "D4"; //wildlife quantity
        public const string SiteNumber = "SiteNumber";
        public const string Time = "Time";
        public const string Direction = "Direction";
        public const string YN = "YN";
        public const string Phone = "Phone";
        public const string Alphanumeric = "Alphanumeric";

        private readonly Dictionary<string, RegexInfo> _regexInfos;

        public RegexDefs()
        {
            _regexInfos = new Dictionary<string, RegexInfo>();

            _regexInfos.Add(Email, new RegexInfo { Regex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Wrong email address" });

            _regexInfos.Add(GpsCoords, new RegexInfo { Regex = @"^\-?\d{1,5}(\.\d{1,9})?$", ErrorMessage = "Value must be a number of less than 6 digits optionally with maximum 9 decimal digits" });
            _regexInfos.Add(Offset, new RegexInfo { Regex = @"^\-?\d{1,4}(\.\d{1,3})?$", ErrorMessage = "Value must be a number of less than 5 digits optionally with maximum 3 decimal digits" });
            _regexInfos.Add(DollarValue, new RegexInfo { Regex = @"^\-?\d{1,7}(\.\d{1,2})?$", ErrorMessage = "Value must be a number of less than 8 digits optionally with maximum 2 decimal digits" });
            _regexInfos.Add(Volume, new RegexInfo { Regex = @"^\-?\d{1,4}(\.\d{1,2})?$", ErrorMessage = "Value must be a number of less than 5 digits optionally with maximum 2 decimal digits" });
            _regexInfos.Add(Quantity, new RegexInfo { Regex = @"^\-?\d{1,4}$", ErrorMessage = "Value must be a number of less than 5 digits" });

            _regexInfos.Add(SiteNumber, new RegexInfo { Regex = @"^[ABDLRSTWX]\d{4}\d{0,2}$", ErrorMessage = "Value must start with one of these 9 [ABDLRSTWX] letters followed by 4 to 6 digits" });

            //_regexInfos.Add(F, new RegexInfo { Regex = @"^[F]$", ErrorMessage = "Value must be F" });
            _regexInfos.Add(Time, new RegexInfo { Regex = @"^(0[0-9]|1[0-9]|2[0-3]|[0-9]):[0-5][0-9]$", ErrorMessage = "Value must be in time format such as 21:35" });
            _regexInfos.Add(Direction, new RegexInfo { Regex = @"^[NSEW]$", ErrorMessage = "Value must be one of these 4 [NSEW] letters" });
            _regexInfos.Add(YN, new RegexInfo { Regex = @"^[YN]$", ErrorMessage = "Value must be Y or N" });
            _regexInfos.Add(Phone, new RegexInfo { Regex = @"^(\+0?1\s)?\(?\d{3}\)?[\s.-]\d{3}[\s.-]\d{4}$", ErrorMessage = "Value must follow phone number format" });
            
            _regexInfos.Add(Alphanumeric, new RegexInfo { Regex = @"^[a-zA-Z0-9]*$", ErrorMessage = "Value must be alphanumeric" });

            //_regexInfos.Add(W, new RegexInfo { Regex = @"^[W]$", ErrorMessage = "Value must be W" });
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
