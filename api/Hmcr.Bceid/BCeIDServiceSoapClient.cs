using System;
using System.Collections.Generic;
using System.Text;

namespace BceidService
{
    public partial class BCeIDServiceSoapClient
    {
        public string Osid { get; set; }
        public string Guid { get; set; }
        public double CacheLifespan { get; set; }
    }
}
