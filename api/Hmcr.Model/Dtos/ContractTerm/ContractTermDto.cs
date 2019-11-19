using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.ServiceArea;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ContractTerm
{
    public class ContractTermDto
    {
        [JsonPropertyName("id")]
        public decimal ContractTermId { get; set; }
        public string ContractName { get; set; }
        public decimal PartyId { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
