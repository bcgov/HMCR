using Hmcr.Model.Dtos.ContractTerm;
using Hmcr.Model.Dtos.SubmissionObject;
using Hmcr.Model.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.Party
{
    public class PartyDto
    {
        public PartyDto()
        {
            ContractTerms = new HashSet<ContractTermDto>();
            SubmissionObjects = new HashSet<SubmissionObjectDto>();
            Users = new HashSet<UserDto>();
        }

        public decimal PartyId { get; set; }
        public Guid? BusinessGuid { get; set; }
        public string BusinessLegalName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public decimal? BusinessNumber { get; set; }
        public string PartyType { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual ICollection<ContractTermDto> ContractTerms { get; set; }
        public virtual ICollection<SubmissionObjectDto> SubmissionObjects { get; set; }
        public virtual ICollection<UserDto> Users { get; set; }
    }
}
