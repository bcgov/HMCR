using Hmcr.Model.Dtos.ContractTerm;
using Hmcr.Model.Dtos.ServiceArea;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.ContractServiceArea
{
    public class ContractServiceAreaDto
    {
        public decimal ContractServiceAreaId { get; set; }
        public decimal ContractTermId { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual ContractTermDto ContractTerm { get; set; }
        public virtual ServiceAreaDto ServiceArea { get; set; }
    }
}
