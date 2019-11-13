using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.ServiceAreaUser
{
    public class ServiceAreaUserDto
    {
        public decimal ServiceAreaUserId { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public decimal SystemUserId { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual ServiceAreaDto ServiceArea { get; set; }
        public virtual UserDto User { get; set; }
    }
}
