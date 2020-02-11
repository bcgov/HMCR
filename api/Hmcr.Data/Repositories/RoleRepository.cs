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
        Task<PagedDto<RoleSearchDto>> GetRolesAync(string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy, string direction);
        Task<RoleDto> GetRoleAsync(decimal roleId);
        Task<HmrRole> CreateRoleAsync(RoleCreateDto role);
        Task UpdateRoleAsync(RoleUpdateDto role);
        Task DeleteRoleAsync(RoleDeleteDto role);
        Task<bool> DoesNameExistAsync(string name);
        IAsyncEnumerable<RoleDto> FindInternalOnlyRolesAsync(IEnumerable<decimal> roleIds);
    }
    public class RoleRepository : HmcrRepositoryBase<HmrRole>, IRoleRepository
    {
        private IUserRoleRepository _userRoleRepo;

        public RoleRepository(AppDbContext dbContext, IMapper mapper, IUserRoleRepository userRoleRepo)
            : base(dbContext, mapper)
        {
            _userRoleRepo = userRoleRepo;
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

        public async Task<PagedDto<RoleSearchDto>> GetRolesAync(string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy, string direction)
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
                    : query.Where(x => x.EndDate != null && x.EndDate <= DateTime.Today);
            }

            var pagedEntity = await Page<HmrRole, HmrRole>(query, pageSize, pageNumber, orderBy, direction);

            var roles = Mapper.Map<IEnumerable<RoleSearchDto>>(pagedEntity.SourceList);

            // Find out which roles are in use
            await foreach (var roleId in FindRolesInUseAync(roles.Select(x => x.RoleId))){
                roles.FirstOrDefault(x => x.RoleId == roleId).IsReferenced = true;
            }

            var pagedDTO = new PagedDto<RoleSearchDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = pagedEntity.TotalCount,
                SourceList = roles,
                OrderBy = orderBy,
                Direction = direction
            };

            return pagedDTO;
        }

        public async Task<RoleDto> GetRoleAsync(decimal roleId)
        {
            var roleEntity = await DbSet.AsNoTracking()
                    .Include(x => x.HmrRolePermissions)
                    .Include(x => x.HmrUserRoles)
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

            role.IsReferenced = await _userRoleRepo.IsRoleInUseAsync(role.RoleId);

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
        
        private async IAsyncEnumerable<decimal> FindRolesInUseAync(IEnumerable<decimal> roleIds)
        {
            foreach (var roleId in roleIds)
            {
                if (await _userRoleRepo.IsRoleInUseAsync(roleId))
                    yield return roleId;
            }
        }

        public async IAsyncEnumerable<RoleDto> FindInternalOnlyRolesAsync(IEnumerable<decimal> roleIds)
        {
            foreach (var roleId in roleIds)
            {
                var role = await GetByIdAsync<RoleDto>(roleId);

                if (role.IsInternal)
                    yield return role;
            }
        }
    }
}
