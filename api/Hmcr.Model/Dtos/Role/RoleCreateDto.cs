using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.Role
{
    public class RoleCreateDto : IRoleSaveDto
    {
        public RoleCreateDto()
        {
            Permissions = new List<decimal>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? EndDate { get; set; }

        public IList<decimal> Permissions { get; set; }
    }
}
