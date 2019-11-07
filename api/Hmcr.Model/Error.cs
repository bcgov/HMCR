using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Hmcr.Model
{
    public class Error
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize<Error>(this, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
