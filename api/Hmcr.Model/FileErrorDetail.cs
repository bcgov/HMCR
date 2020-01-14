using Hmcr.Model.Utils;
using System.Collections.Generic;
using System.Text.Json;

namespace Hmcr.Model
{
    public class FileErrorDetail
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public List<FieldMessage> FieldMessages { get; set; }

        public FileErrorDetail(Dictionary<string, List<string>> errros)
        {
            FieldMessages = new List<FieldMessage>();

            foreach (var error in errros)
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
            return JsonSerializer.Serialize<FileErrorDetail>(this, _jsonOptions);
        }
    }
}
