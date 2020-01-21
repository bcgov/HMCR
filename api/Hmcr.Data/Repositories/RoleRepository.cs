using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.Role;
using Hmcr.Model.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IRoleRepository : IHmcrRepositoryBase<HmrRole>
    {
        Task<int> CountActiveRoleIdsAsync(IEnumerable<decimal> roles);
        Task<IEnumerable<RoleDto>> GetActiveRolesAsync();
        Task<PagedDto<RoleSearchDto>> GetRolesAync(string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy);
        Task<RoleDto> GetRoleAsync(decimal roleId);
        Task<HmrRole> CreateRoleAsync(RoleCreateDto role);
        Task UpdateRoleAsync(RoleUpdateDto role);
        Task DeleteRoleAsync(RoleDeleteDto role);
        Task<bool> DoesNameExistAsync(string name);
        IAsyncEnumerable<RoleDto> FindInternalOnlyRolesAsync(IEnumerable<decimal> roleIds);
    }
    public class RoleRepository : HmcrRepositoryBase<HmrRole>, IRoleRepository
    {
        public RoleRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<int> CountActiveRoleIdsAsync(IEnumerable<decimal> roles)
        {
            return await DbSet.CountAsync(r => roles.Contains(r.RoleId) && (r.EndDate == null || r.EndDate > DateTime.Today));
        }

        public async Task<IEnumerable<RoleDto>> GetActiveRolesAsync()
        {
            var roleEntity = await DbSet.AsNoTracking()
                .Where(x => x.EndDate == null || x.EndDate > DateTime.Today)
                .ToListAsync();

            return Mapper.Map<IEnumerable<RoleDto>>(roleEntity);
        } 

        public async Task<PagedDto<RoleSearchDto>> GetRolesAync(string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy)
        {
            var query = DbSet.AsNoTracking();

            if (searchText.IsNotEmpty())
            {
                query = query
                    .Where(x => x.Name.Contains(searchText) || x.Description.Contains(searchText));
            }

            if (isActive != null)
            {
                query = (bool)isActive
                    ? query.Where(x => x.EndDate == null || x.EndDate > DateTime.Today)
                    : query.Where(x => x.EndDate != null || x.EndDate <= DateTime.Today.AddDays(1));
            }

            query = query.Include(x => x.HmrUserRoles);

            var pagedEntity = await Page<HmrRole, HmrRole>(query, pageSize, pageNumber, orderBy);

            var roles = Mapper.Map<IEnumerable<RoleSearchDto>>(pagedEntity.SourceList);

            foreach (var role in roles)
            {
                role.InUse = pagedEntity.SourceList.Any(r => r.RoleId == role.RoleId && r.HmrUserRoles.Count > 0);
            }

            var pagedDTO = new PagedDto<RoleSearchDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = pagedEntity.TotalCount,
                SourceList = roles
            };

            return pagedDTO;
        }

        public async Task<RoleDto> GetRoleAsync(decimal roleId)
        {
            var roleEntity = await DbSet.AsNoTracking()
                    .Include(x => x.HmrRolePermissions)
                    .FirstOrDefaultAsync(x => x.RoleId == roleId);

            if (roleEntity == null)
                return null;

            var role = Mapper.Map<RoleDto>(roleEntity);

            var permissionIds =
                roleEntity
                .HmrRolePermissions
                .Where(x => x.EndDate == null || x.EndDate > DateTime.Today) 
                .Select(x => x.PermissionId)
                .ToList();

            role.Permissions = permissionIds;

            return role;
        }

        public async Task<HmrRole> CreateRoleAsync(RoleCreateDto role)
        {
            var roleEntity = await AddAsync(role);

            foreach (var permission in role.Permissions)
            {
                roleEntity.HmrRolePermissions
                    .Add(new HmrRolePermission
                    {
                        PermissionId = permission
                    });
            }

            return roleEntity;
        }

        public async Task UpdateRoleAsync(RoleUpdateDto role)
        {
            //remove time portion
            role.EndDate = role.EndDate?.Date;

            var roleEntity = await DbSet
                    .Include(x => x.HmrRolePermissions)
                    .FirstAsync(x => x.RoleId == role.RoleId);

            Mapper.Map(role, roleEntity);

            SyncPermissions(role, roleEntity);
        }

        private void SyncPermissions(RoleUpdateDto role, HmrRole roleEntity)
        {
            var permissionsToDelete =
                roleEntity.HmrRolePermissions.Where(x => !role.Permissions.Contains(x.PermissionId)).ToList();

            for (var i = permissionsToDelete.Count() - 1; i >= 0; i--)
            {
                DbContext.Remove(permissionsToDelete[i]);
            }

            var existingPermissionIds = roleEntity.HmrRolePermissions.Select(x => x.PermissionId);

            var newPermissionIds = role.Permissions.Where(x => !existingPermissionIds.Contains(x));

            foreach (var permissionId in newPermissionIds)
            {
                roleEntity.HmrRolePermissions
                    .Add(new HmrRolePermission
                    {
                        PermissionId = permissionId,
                        RoleId = roleEntity.RoleId
                    });
            }
        }

        public async Task DeleteRoleAsync(RoleDeleteDto role)
        {
            //remove time portion
            role.EndDate = role.EndDate?.Date;

            var roleEntity = await DbSet
                .FirstAsync(x => x.RoleId == role.RoleId);

            Mapper.Map(role, roleEntity);
        }

        public async Task<bool> DoesNameExistAsync(string name)
        {
            return await DbSet.AnyAsync(x => x.Name == name);
        }

        public async IAsyncEnumerable<RoleDto> FindInternalOnlyRolesAsync(IEnumerable<decimal> roleIds)
        {
            foreach (var roleId in roleIds)
            {
                var role = await GetByIdAsync<RoleDto>(roleId);

                if (role.IsInternal == "Y")
                    yield return role;
            }
        }
    }
}
