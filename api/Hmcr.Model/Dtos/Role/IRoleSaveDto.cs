using System.Collections.Generic;

namespace Hmcr.Model.Dtos.Role
{
    public interface IRoleSaveDto
    {
        IList<decimal> Permissions { get; set; }
    }
}
