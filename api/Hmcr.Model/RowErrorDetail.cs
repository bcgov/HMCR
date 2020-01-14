using Hmcr.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Hmcr.Model
{
    public class RowErrorDetail
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public int RowNumber { get; set; }

        public List<FieldMessage> FieldMessages { get; set; }

        public RowErrorDetail(int rowNumber, Dictionary<string, List<string>> errros)
        {
            RowNumber = rowNumber;

            FieldMessages = new List<FieldMessage>();

            foreach(var error in errros)
            {
                FieldMessages.Add(new FieldMessage
                {
                    Field = error.Key.WordToWords(),
                    Messages = error.Value
                });
            }
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize<RowErrorDetail>(this, _jsonOptions);
        }
    }
}
