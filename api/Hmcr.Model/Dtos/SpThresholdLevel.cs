using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos
{
    public class SpThresholdLevel
    {
        public int Error { get; set; }
        public int Warning { get; set; }

        public SpThresholdLevel(string codeText)
        {
            if (codeText == null)
            {
                throw new Exception("Failed parsing THRSHLD_SP_VAR value");
            }

            var csv = codeText.Split(',');

            if (csv.Length != 2)
            {
                throw new Exception($"Cannot parse THRSHLD_SP_VAR value {codeText}");
            }

            if (!int.TryParse(csv[0], out int warning))
            {
                throw new Exception($"Cannot parse the first value of THRSHLD_SP_VAR value {csv[0]}");
            }
            else
            {
                Warning = warning;
            }

            if (!int.TryParse(csv[1], out int error))
            {
                throw new Exception($"Cannot parse the second value of THRSHLD_SP_VAR value {csv[1]}");
            }
            else
            {
                Error = error;
            }
        }
    }
}
