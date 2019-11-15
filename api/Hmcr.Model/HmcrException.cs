using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model
{
    public class HmcrException : Exception
    {
        public HmcrException(string message)
            : base(message)
        {
        }
    }
}
