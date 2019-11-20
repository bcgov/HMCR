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
    }
    public class RoleService : IRoleService
    {
        private IRoleRepository _roleRepo;

        public RoleService(IRoleRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }
        public async Task<IEnumerable<RoleDto>> GetActiveRolesAsync()
        {
            return await _roleRepo.GetActiveRolesAsync();
        }
    }
}
