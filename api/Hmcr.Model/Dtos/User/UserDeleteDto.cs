using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.User
{
    public class UserDeleteDto
    {
        [JsonPropertyName("id")]
        public decimal SystemUserId { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
