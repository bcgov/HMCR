﻿using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.Role;
using Hmcr.Model.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetActiveRolesAsync();
        Task<int> CountActiveRoleIdsAsync(IEnumerable<decimal> roles);
        Task<PagedDto<RoleSearchDto>> GetRolesAync(string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy, string direction);
        Task<RoleDto> GetRoleAsync(decimal roleId);
        Task<(decimal RoleId, Dictionary<string, List<string>> Errors)> CreateRoleAsync(RoleCreateDto role);
        Task<(bool NotFound, Dictionary<string, List<string>> Errors)> UpdateRoleAsync(RoleUpdateDto role);
        Task<(bool NotFound, Dictionary<string, List<string>> Errors)> DeleteRoleAsync(RoleDeleteDto role);
    }
    public class RoleService : IRoleService
    {
        private IRoleRepository _roleRepo;
        IUserRoleRepository _userRoleRepo;
        private IUnitOfWork _unitOfWork;
        private IFieldValidatorService _validator;
        private IPermissionRepository _permRepo;

        public RoleService(IRoleRepository roleRepo, IUserRoleRepository userRoleRepo, IPermissionRepository permRepo, IUnitOfWork unitOfWork, IFieldValidatorService validator)
        {
            _roleRepo = roleRepo;
            _userRoleRepo = userRoleRepo;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _permRepo = permRepo;
        }

        public async Task<int> CountActiveRoleIdsAsync(IEnumerable<decimal> roles)
        {
            return await _roleRepo.CountActiveRoleIdsAsync(roles);
        }

        public async Task<(decimal RoleId, Dictionary<string, List<string>> Errors)> CreateRoleAsync(RoleCreateDto role)
        {
            var errors = await ValidateRoleDtoAsync(role);

            if (role.Name.IsNotEmpty())
            {
                if (await _roleRepo.DoesNameExistAsync(role.Name))
                {
                    errors.AddItem(Fields.Name, $"Name [{role.Name}] already exists.");
                }
            }

            if (errors.Count > 0)
            {
                return (0, errors);
            }

            var roleEntity = await _roleRepo.CreateRoleAsync(role);

            await _unitOfWork.CommitAsync();

            return (roleEntity.RoleId, errors);
        }

        private async Task<Dictionary<string, List<string>>> ValidateRoleDtoAsync<T>(T role) where T : IRoleSaveDto
        {
            var errors = new Dictionary<string, List<string>>();

            _validator.Validate(Entities.Role, role, errors);

            var permissionCount = await _permRepo.CountActivePermissionIdsAsnyc(role.Permissions);
            if (permissionCount != role.Permissions.Count)
            {
                errors.AddItem(Fields.PermissionId, $"Some of the permission IDs are invalid or inactive.");
            }

            return errors;
        }

        public async Task<(bool NotFound, Dictionary<string, List<string>> Errors)> DeleteRoleAsync(RoleDeleteDto role)
        {
            var roleFromDb = await GetRoleAsync(role.RoleId);

            if (roleFromDb == null)
            {
                return (true, null);
            }

            var errors = new Dictionary<string, List<string>>();

            _validator.Validate(Entities.Role, role, errors);

            if (errors.Count > 0)
            {
                return (false, errors);
            }

            await _roleRepo.DeleteRoleAsync(role);

            await _unitOfWork.CommitAsync();

            return (false, errors);
        }

        public async Task<IEnumerable<RoleDto>> GetActiveRolesAsync()
        {
            return await _roleRepo.GetActiveRolesAsync();
        }

        public async Task<RoleDto> GetRoleAsync(decimal roleId)
        {
            return await _roleRepo.GetRoleAsync(roleId);
        }

        public async Task<PagedDto<RoleSearchDto>> GetRolesAync(string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy, string direction)
        {
            return await _roleRepo.GetRolesAync(searchText, isActive, pageSize, pageNumber, orderBy, direction);
        }

        public async Task<(bool NotFound, Dictionary<string, List<string>> Errors)> UpdateRoleAsync(RoleUpdateDto role)
        {
            var roleFromDb = await GetRoleAsync(role.RoleId);

            if (roleFromDb == null)
            {
                return (true, null);
            }

            var errors = await ValidateRoleDtoAsync(role);

            if (role.Name != roleFromDb.Name)
            {
                if (await _roleRepo.DoesNameExistAsync(role.Name))
                {
                    errors.AddItem(Fields.Username, $"The role name [{role.Name}] already exists.");
                }
            }

            if(role.IsInternal != roleFromDb.IsInternal)
            {
                var isRoleReferenced = await _userRoleRepo.IsRoleInUseAsync(role.RoleId);

                if (isRoleReferenced)
                {
                    errors.AddItem(Fields.IsInternal, $"Cannot set role Internal to [{role.IsInternal}] because role is in use.");
                }
            }

            if (errors.Count > 0)
            {
                return (false, errors);
            }

            await _roleRepo.UpdateRoleAsync(role);

            await _unitOfWork.CommitAsync();

            return (false, errors);
        }
    }
}
