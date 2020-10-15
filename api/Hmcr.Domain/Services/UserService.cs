using AutoMapper;
using Hmcr.Bceid;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.Role;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IUserService
    {
        Task<UserCurrentDto> GetCurrentUserAsync();
        Task<PagedDto<UserSearchDto>> GetUsersAsync(decimal[]? serviceAreas, string[]? userTypes, string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy, string direction);
        Task<UserDto> GetUserAsync(decimal systemUserId);
        Task<UserBceidAccountDto> GetBceidAccountAsync(string username, string userType);
        Task<(decimal SystemUserId, Dictionary<string, List<string>> Errors)> CreateUserAsync(UserCreateDto user);
        Task<(bool NotFound, Dictionary<string, List<string>> Errors)> UpdateUserAsync(UserUpdateDto user);
        Task<(bool NotFound, Dictionary<string, List<string>> Errors)> DeleteUserAsync(UserDeleteDto user);
        Task<HmrSystemUser> GetActiveUserEntityAsync(Guid userGuid);
        Task UpdateUserFromBceidAsync(Guid userGuid, string username, string userType, long concurrencyControlNumber);
    }
    public class UserService : IUserService
    {
        private IUserRepository _userRepo;
        private IPartyRepository _partyRepo;
        private IServiceAreaRepository _serviceAreaRepo;
        private IRoleRepository _roleRepo;
        private IUnitOfWork _unitOfWork;
        private HmcrCurrentUser _currentUser;
        private IFieldValidatorService _validator;
        private IBceidApi _bceid;
        private IMapper _mapper;
        private ILogger _logger;

        public UserService(IUserRepository userRepo, IPartyRepository partyRepo, IServiceAreaRepository serviceAreaRepo, IRoleRepository roleRepo,
            IUnitOfWork unitOfWork, HmcrCurrentUser currentUser, IFieldValidatorService validator, IBceidApi bceid, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _partyRepo = partyRepo;
            _serviceAreaRepo = serviceAreaRepo;
            _roleRepo = roleRepo;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _validator = validator;
            _bceid = bceid;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserCurrentDto> GetCurrentUserAsync()
        {
            return await _userRepo.GetCurrentUserAsync();
        }

        public async Task<HmrSystemUser> GetActiveUserEntityAsync(Guid userGuid)
        {
            return await _userRepo.GetActiveUserEntityAsync(userGuid);
        }

        public async Task<PagedDto<UserSearchDto>> GetUsersAsync(decimal[]? serviceAreas, string[]? userTypes, string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy, string direction)
        {
            return await _userRepo.GetUsersAsync(serviceAreas, userTypes, searchText, isActive, pageSize, pageNumber, orderBy, direction);
        }

        public async Task<UserDto> GetUserAsync(decimal systemUserId)
        {
            return await _userRepo.GetUserAsync(systemUserId);
        }

        public async Task<UserBceidAccountDto> GetBceidAccountAsync(string username, string userType)
        {
            var (error, account) = await _bceid.GetBceidAccountCachedAsync(null, username, userType, _currentUser.UserGuid, _currentUser.UserType);

            if (string.IsNullOrEmpty(error))
            {
                return _mapper.Map<UserBceidAccountDto>(account);
            }

            return null;
        }

        public async Task<(decimal SystemUserId, Dictionary<string, List<string>> Errors)> CreateUserAsync(UserCreateDto user)
        {
            var (error, account) = await _bceid.GetBceidAccountCachedAsync(null, user.Username, user.UserType, _currentUser.UserGuid, _currentUser.UserType);
            if (error.IsNotEmpty())
            {
                throw new HmcrException($"Unable to retrieve User[{user.Username}]-[{user.UserType}] from BCeID Service.");
            }

            user.Email = account.Email;

            var errors = await ValidateUserDtoAsync(user);

            if (await _userRepo.DoesUsernameExistAsync(user.Username, user.UserType))
            {
                errors.AddItem(Fields.Username, $"Username [{user.Username}]-[{user.UserType}] already exists.");
            }

            if (errors.Count > 0)
            {
                return (0, errors);
            }

            var userEntity = await _userRepo.CreateUserAsync(user, account);
            _unitOfWork.Commit();

            return (userEntity.SystemUserId, errors);
        }

        public async Task<(bool NotFound, Dictionary<string, List<string>> Errors)> UpdateUserAsync(UserUpdateDto user)
        {
            var userFromDb = await GetUserAsync(user.SystemUserId);

            if (userFromDb == null)
            {
                return (true, null);
            }

            var errors = await ValidateUserDtoAsync(user);

            if (errors.Count > 0)
            {
                return (false, errors);
            }

            await _userRepo.UpdateUserAsync(user);
            _unitOfWork.Commit();

            return (false, errors);
        }

        private async Task<Dictionary<string, List<string>>> ValidateUserDtoAsync<T>(T user) where T : IUserSaveDto
        {
            var entityName = Entities.User;
            var errors = new Dictionary<string, List<string>>();

            _validator.Validate(entityName, user, errors);

            var serviceAreaCount = await _serviceAreaRepo.CountServiceAreaNumbersAsync(user.ServiceAreaNumbers);
            if (serviceAreaCount != user.ServiceAreaNumbers.Count)
            {
                errors.AddItem(Fields.ServiceAreaNumber, $"Some of the user's service areas are invalid.");
            }

            if(user.UserType != UserTypeDto.INTERNAL && serviceAreaCount == 0)
            {
                errors.AddItem(Fields.ServiceAreaNumber, $"Service area must be set for the BCeID user.");
            }

            var roleCount = await _roleRepo.CountActiveRoleIdsAsync(user.UserRoleIds);
            if (roleCount != user.UserRoleIds.Count)
            {
                errors.AddItem(Fields.RoleId, $"Some of the user's role IDs are invalid or inactive.");
            }

            if (!_currentUser.UserInfo.IsSystemAdmin)
            {
                foreach (var roleId in user.UserRoleIds)
                {
                    await CheckIfCurrentUserHasAllThePermissions(roleId, errors);
                }
            }

            if (user.UserType != UserTypeDto.INTERNAL)
            {
                await foreach(var role in _roleRepo.FindInternalOnlyRolesAsync(user.UserRoleIds))
                {
                    errors.AddItem(Fields.RoleId, $"{role.Description} cannot be assigned to business user");
                }
            }

            return errors;
        }

        private async Task CheckIfCurrentUserHasAllThePermissions(decimal roleId, Dictionary<string, List<string>> errors)
        {
            var permissionsInRole = await _roleRepo.GetRolePermissionsAsync(roleId);

            foreach (var permission in permissionsInRole.Permissions)
            {
                if (!_currentUser.UserInfo.Permissions.Any(x => x == permission))
                {
                    var role = await _roleRepo.GetRoleAsync(roleId);
                    errors.AddItem(Fields.RoleId, $"User is not authorized to assign the role {permissionsInRole.RoleName}");
                    return;
                }
            }
        }

        public async Task<(bool NotFound, Dictionary<string, List<string>> Errors)> DeleteUserAsync(UserDeleteDto user)
        {
            //todo: if user has no log-in history, we can delete user instead of deactivating it. 
            var userFromDb = await GetUserAsync(user.SystemUserId);

            if (userFromDb == null)
            {
                return (true, null);
            }

            var errors = new Dictionary<string, List<string>>();

            _validator.Validate(Entities.User, user, errors);

            if (errors.Count > 0)
            {
                return (false, errors);
            }

            await _userRepo.DeleteUserAsync(user);

            _unitOfWork.Commit();

            return (false, errors);
        }

        public async Task UpdateUserFromBceidAsync(Guid userGuid, string username, string userType, long concurrencyControlNumber)
        {
            var (error, account) = await _bceid.GetBceidAccountCachedAsync(userGuid, username, userType, _currentUser.UserGuid, _currentUser.UserType);

            if (error.IsNotEmpty())
            {
                throw new HmcrException($"Unable to retrieve User [{userGuid.ToString("N")} ({userType})] from BCeID Service.");
            }

            await _userRepo.UpdateUserFromBceidAsync(account, concurrencyControlNumber);
        }
    }
}
