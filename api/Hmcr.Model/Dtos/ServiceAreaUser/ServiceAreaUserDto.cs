﻿using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ServiceAreaUser
{
    public class ServiceAreaUserDto
    {
        [JsonPropertyName("id")]
        public decimal ServiceAreaUserId { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public decimal SystemUserId { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
