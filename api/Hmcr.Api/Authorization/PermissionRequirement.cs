using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Hmcr.Api.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public IEnumerable<string> RequiredPermissions { get; }

        public PermissionRequirement(params string[] permissions)
        {
            RequiredPermissions = permissions;
        }
    }
}
