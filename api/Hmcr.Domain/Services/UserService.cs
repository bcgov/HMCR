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
        Task<bool> ProcessFirstUserLoginAsync();
        Task<PagedDto<UserSearchDto>> GetUsersAsync(decimal[]? serviceAreas, string[]? userTypes, string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy);
        Task<UserDto> GetUserAsync(decimal systemUserId);
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

        public UserService(IUserRepository userRepo, IPartyRepository partyRepo, IServiceAreaRepository serviceAreaRepo, IRoleRepository roleRepo,
            IUnitOfWork unitOfWork, HmcrCurrentUser currentUser, IFieldValidatorService validator)
        {
            _userRepo = userRepo;
            _partyRepo = partyRepo;
            _serviceAreaRepo = serviceAreaRepo;
            _roleRepo = roleRepo;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _validator = validator;
        }

        public async Task<UserCurrentDto> GetCurrentUserAsync()
        {
            return await _userRepo.GetCurrentUserAsync();
        }

        public async Task<bool> ProcessFirstUserLoginAsync()
        {
            var userEntity = await _userRepo.GetCurrentActiveUserEntityAsync();

            if (userEntity == null)
            {
                return false;
            }

            if (userEntity.UserGuid == null)
            {
                if (userEntity.UserType == _currentUser.UserType)
                {
                    UpdateUserEntity(userEntity);
                    CreatePartyEntityIfNecessaryAsync();
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    throw new HmcrException($"User[{_currentUser.UniversalId}] exists in the user table with a wrong user type [{_currentUser.UserType}].");
                }
            }

            return true;
        }

        private void UpdateUserEntity(HmrSystemUser userEntity)
        {
            userEntity.UserGuid = _currentUser.UserGuid;
            userEntity.BusinessGuid = _currentUser.BusinessGuid;
            userEntity.BusinessLegalName = _currentUser.BusinessLegalName;
            userEntity.UserType = _currentUser.UserType;
        }

        private async void CreatePartyEntityIfNecessaryAsync()
        {
            if (_currentUser.UserType == UserTypeDto.INTERNAL)
                return;

            var partyEntity = await _partyRepo.GetPartyEntityByGuidAsync(_currentUser.BusinessGuid);

            if (partyEntity != null)
                return;

            var party = new PartyDto
            {
                BusinessGuid = _currentUser.BusinessGuid,
                BusinessLegalName = _currentUser.BusinessLegalName.Trim(),
                BusinessNumber = Convert.ToDecimal(_currentUser.BusinessNumber),
                DisplayName = _currentUser.BusinessLegalName.Trim()
            };

            await _partyRepo.AddAsync(party);
        }

        public async Task<PagedDto<UserSearchDto>> GetUsersAsync(decimal[]? serviceAreas, string[]? userTypes, string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy)
        {
            return await _userRepo.GetUsersAsync(serviceAreas, userTypes, searchText, isActive, pageSize, pageNumber, orderBy);
        }

        public async Task<UserDto> GetUserAsync(decimal systemUserId)
        {
            return await _userRepo.GetUserAsync(systemUserId);
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
