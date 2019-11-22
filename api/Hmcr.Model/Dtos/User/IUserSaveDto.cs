using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.User
{
    public interface IUserSaveDto
    {
        string UserType { get; set; }
        string Username { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        DateTime? EndDate { get; set; }
        string UserDirectory { get; set; }

        IList<decimal> ServiceAreaNumbers { get; set; }
        IList<decimal> UserRoleIds { get; set; }
    }
}
