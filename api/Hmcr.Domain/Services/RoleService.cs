using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.Role;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetActiveRolesAsync();
        Task<int> CountActiveRoleIdsAsync(IEnumerable<decimal> roles);
    }
    public class RoleService : IRoleService
    {
        private IRoleRepository _roleRepo;

        public RoleService(IRoleRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<int> CountActiveRoleIdsAsync(IEnumerable<decimal> roles)
        {
            return await _roleRepo.CountActiveRoleIdsAsync(roles);
        }
        public async Task<IEnumerable<RoleDto>> GetActiveRolesAsync()
        {
            return await _roleRepo.GetActiveRolesAsync();
        }
    }
}
