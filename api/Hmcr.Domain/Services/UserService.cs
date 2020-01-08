using AutoMapper;
using Hmcr.Bceid;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IUserService
    {
        Task<UserCurrentDto> GetCurrentUserAsync();
        Task<bool> ValidateUserLoginAsync();
        Task<PagedDto<UserSearchDto>> GetUsersAsync(decimal[]? serviceAreas, string[]? userTypes, string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy);
        Task<UserDto> GetUserAsync(decimal systemUserId);
        Task<UserBceidAccountDto> GetBceidAccountAsync(string username, string userType);
        Task<(decimal SystemUserId, Dictionary<string, List<string>> Errors)> CreateUserAsync(UserCreateDto user);
        Task<(bool NotFound, Dictionary<string, List<string>> Errors)> UpdateUserAsync(UserUpdateDto user);
        Task<(bool NotFound, Dictionary<string, List<string>> Errors)> DeleteUserAsync(UserDeleteDto user);
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

        public UserService(IUserRepository userRepo, IPartyRepository partyRepo, IServiceAreaRepository serviceAreaRepo, IRoleRepository roleRepo,
            IUnitOfWork unitOfWork, HmcrCurrentUser currentUser, IFieldValidatorService validator, IBceidApi bceid, IMapper mapper)
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
        }

        public async Task<UserCurrentDto> GetCurrentUserAsync()
        {
            return await _userRepo.GetCurrentUserAsync();
        }

        //todo: allow same username for different userType
        public async Task<bool> ValidateUserLoginAsync()
        {
            var userEntity = await _userRepo.GetCurrentActiveUserEntityAsync();

            if (userEntity == null)
            {
                return false;
            }

            if (userEntity.UserGuid == null)
            {
                if (userEntity.UserType != _currentUser.UserType)
                {
                    throw new HmcrException($"User[{_currentUser.UniversalId}] exists in the user table with a wrong user type [{_currentUser.UserType}].");
                }
            }
            else if (userEntity.UserGuid != _currentUser.UserGuid)
            {
                throw new HmcrException($"User[{_currentUser.UniversalId}] exists in the user table with a wrong User Guid. Login UserGuid: [{_currentUser.UserGuid}] Registered UserGuid: [{userEntity.UserGuid}].");
            }

            return true;
        }

        public async Task<PagedDto<UserSearchDto>> GetUsersAsync(decimal[]? serviceAreas, string[]? userTypes, string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy)
        {
            return await _userRepo.GetUsersAsync(serviceAreas, userTypes, searchText, isActive, pageSize, pageNumber, orderBy);
        }

        public async Task<UserDto> GetUserAsync(decimal systemUserId)
        {
            return await _userRepo.GetUserAsync(systemUserId);
        }

        public async Task<UserBceidAccountDto> GetBceidAccountAsync(string username, string userType)
        {
            var (error, account) = await _bceid.GetBceidAccountCachedAsync(username, userType);

            if (string.IsNullOrEmpty(error))
            {
                return _mapper.Map<UserBceidAccountDto>(account);
            }

            return null;
        }

        public async Task<(decimal SystemUserId, Dictionary<string, List<string>> Errors)> CreateUserAsync(UserCreateDto user)
        {
            var errors = await ValidateUserDtoAsync(user);

            if (user.Username.IsNotEmpty())
            {
                if (await _userRepo.DoesUsernameExistAsync(user.Username))
                {
                    errors.AddItem(Fields.Username, $"Username [{user.Username}] already exists.");
                }
            }

            if (errors.Count > 0)
            {
                return (0, errors);
            }

            var userEntity = await _userRepo.CreateUserAsync(user);

            await _unitOfWork.CommitAsync();

            var (error, account) = await _bceid.GetBceidAccountCachedAsync(user.Username, user.UserType);

            if (string.IsNullOrEmpty(error))
            {
                _userRepo.ProcessFirstUserLogin(account);
            }
            else
            {
                throw new HmcrException($"Unable to retrieve User[{user.Username}] of type [{user.UserType}] from BCeID Service.");
            }

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

            if (userFromDb.HasLogInHistory)
            {
                ValidateUserWithLoginHistory(user, userFromDb, errors);
            }

            if (user.Username != userFromDb.Username)
            {
                if (await _userRepo.DoesUsernameExistAsync(user.Username))
                {
                    errors.AddItem(Fields.Username, $"Username [{user.Username}] already exists.");
                }
            }

            if (errors.Count > 0)
            {
                return (false, errors);
            }

            await _userRepo.UpdateUserAsync(user);

            await _unitOfWork.CommitAsync();

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

            var roleCount = await _roleRepo.CountActiveRoleIdsAsync(user.UserRoleIds);
            if (roleCount != user.UserRoleIds.Count)
            {
                errors.AddItem(Fields.RoleId, $"Some of the user's role IDs are invalid or inactive.");
            }

            return errors;
        }

        private void ValidateUserWithLoginHistory(UserUpdateDto user, UserDto userFromDb, Dictionary<string, List<string>> errors)
        {
            if (user.UserType != userFromDb.UserType)
            {
                errors.AddItem(Fields.UserType, $"The {Fields.UserType} field cannot be changed because the user has log-in history.");
            }

            if (user.UserDirectory != userFromDb.UserDirectory)
            {
                errors.AddItem(Fields.UserDirectory, $"The {Fields.UserDirectory} field cannot be changed because the user has log-in history.");
            }

            if (user.Username != userFromDb.Username)
            {
                errors.AddItem(Fields.Username, $"The {Fields.Username} field cannot be changed because the user has log-in history.");
            }

            if (user.FirstName != userFromDb.FirstName)
            {
                errors.AddItem(Fields.FirstName, $"The {Fields.FirstName} field cannot be changed because the user has log-in history.");
            }

            if (user.LastName != userFromDb.LastName)
            {
                errors.AddItem(Fields.LastName, $"The {Fields.LastName} field cannot be changed because the user has log-in history.");
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

            await _unitOfWork.CommitAsync();

            return (false, errors);
        }
    }
}
