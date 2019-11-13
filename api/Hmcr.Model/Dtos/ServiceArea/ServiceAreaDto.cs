using Hmcr.Model.Dtos.ContractServiceArea;
using Hmcr.Model.Dtos.ContractTerm;
using Hmcr.Model.Dtos.District;
using Hmcr.Model.Dtos.ServiceAreaUser;
using Hmcr.Model.Dtos.SubmissionObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.ServiceArea
{
    public class ServiceAreaDto
    {
        public ServiceAreaDto()
        {
            ContractServiceAreas = new HashSet<ContractServiceAreaDto>();
            ServiceAreaUsers = new HashSet<ServiceAreaUserDto>();
            SubmissionObjects = new HashSet<SubmissionObjectDto>();
        }

        public decimal ServiceAreaId { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public string ServiceAreaName { get; set; }
        public decimal DistrictNumber { get; set; }

        public virtual DistrictDto District { get; set; }
        public virtual ICollection<ContractServiceAreaDto> ContractServiceAreas { get; set; }
        public virtual ICollection<ServiceAreaUserDto> ServiceAreaUsers { get; set; }
        public virtual ICollection<SubmissionObjectDto> SubmissionObjects { get; set; }
    }
}
