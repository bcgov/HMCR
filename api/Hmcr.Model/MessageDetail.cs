using Hmcr.Model.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hmcr.Model
{

    public class MessageDetail
    {
        /// <summary>
        /// ERROR_DETAIL and WARNING_DETAIL are VARCHAR(4000). Exceeding the limit used to
        /// cause a SQL truncation error on save, which failed the submission with an
        /// opaque 'Unexpected Error'.
        /// </summary>
        public const int MaxSerializedLength = 4000;
        private const int MaxSingleMessageLength = 2000;

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
            var json = JsonSerializer.Serialize<MessageDetail>(this, _jsonOptions);

            if (json.Length <= MaxSerializedLength)
                return json;

            return SerializeWithinLimit();
        }

        /// <summary>
        /// Rebuilds the detail keeping as many messages as fit within the column limit,
        /// with a note that the remainder were omitted. Always produces valid JSON.
        /// </summary>
        private string SerializeWithinLimit()
        {
            var limited = new MessageDetail();
            var note = new FieldMessage
            {
                Field = "Note",
                Messages = new List<string>
                {
                    "Additional messages were omitted because the detail exceeded the maximum length the system can store. " +
                    "Please correct the reported items and submit the file again to see any remaining messages."
                }
            };
            limited.FieldMessages.Add(note);

            foreach (var fieldMessage in FieldMessages)
            {
                foreach (var message in fieldMessage.Messages)
                {
                    var targetField = limited.FieldMessages.FirstOrDefault(x => x.Field == fieldMessage.Field && x != note);
                    var fieldWasAdded = false;

                    if (targetField == null)
                    {
                        targetField = new FieldMessage { Field = fieldMessage.Field, Messages = new List<string>() };
                        limited.FieldMessages.Insert(limited.FieldMessages.Count - 1, targetField); //keep the note last
                        fieldWasAdded = true;
                    }

                    var trimmedMessage = message != null && message.Length > MaxSingleMessageLength
                        ? message.Substring(0, MaxSingleMessageLength) + "..."
                        : message;

                    targetField.Messages.Add(trimmedMessage);

                    if (JsonSerializer.Serialize<MessageDetail>(limited, _jsonOptions).Length > MaxSerializedLength)
                    {
                        targetField.Messages.RemoveAt(targetField.Messages.Count - 1);

                        if (fieldWasAdded && targetField.Messages.Count == 0)
                            limited.FieldMessages.Remove(targetField);

                        return JsonSerializer.Serialize<MessageDetail>(limited, _jsonOptions);
                    }
                }
            }

            return JsonSerializer.Serialize<MessageDetail>(limited, _jsonOptions);
        }

    }
}
