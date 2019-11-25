using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.User
{
    public interface IUserSaveDto
    {
        IList<decimal> ServiceAreaNumbers { get; set; }
        IList<decimal> UserRoleIds { get; set; }
    }
}
