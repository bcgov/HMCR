using Hmcr.Model.Dtos.ContractServiceArea;
using Hmcr.Model.Dtos.Party;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.ContractTerm
{
    public class ContractTermDto
    {
        public ContractTermDto()
        {
            ContractServiceAreas = new HashSet<ContractServiceAreaDto>();
        }

        public decimal ContractTermId { get; set; }
        public string ContractName { get; set; }
        public decimal PartyId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual PartyDto Party { get; set; }
        public virtual ICollection<ContractServiceAreaDto> ContractServiceAreas { get; set; }
    }
}
