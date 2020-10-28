using Hmcr.Model.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hmcr.Model
{
    
    public class MessageDetail
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        [JsonPropertyName("fieldMessages")]
        public List<FieldMessage> FieldMessages { get; set; }
        
        public MessageDetail()
        {
            FieldMessages = new List<FieldMessage>();
        }

        public MessageDetail(Dictionary<string, List<string>> messages)
        {
            FieldMessages = new List<FieldMessage>();

            foreach (var message in messages)
            {
                FieldMessages.Add(new FieldMessage
                {
                    Field = message.Key.WordToWords(),
                    Messages = message.Value
                });
            }
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize<MessageDetail>(this, _jsonOptions);
        }

    }
}
