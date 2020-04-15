using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class Counter
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
