using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.Permission;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetActivePermissionsAsync();
    }
    public class PermissionService : IPermissionService
    {
        private IPermissionRepository _permissionRepo;

        public PermissionService(IPermissionRepository permissionRepo)
        {
            _permissionRepo = permissionRepo;
        }
        public async Task<IEnumerable<PermissionDto>> GetActivePermissionsAsync()
        {
            return await _permissionRepo.GetActivePermissionsAsync();
        }
    }
}
