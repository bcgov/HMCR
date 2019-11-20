using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.User;
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
        Task<decimal> CreateUserAsync(UserCreateDto user);
    }
    public class UserService : IUserService
    {
        private IUserRepository _userRepo;
        private IPartyRepository _partyRepo;
        private IUnitOfWork _unitOfWork;
        private HmcrCurrentUser _currentUser;

        public UserService(IUserRepository userRepo, IPartyRepository partyRepo, IUnitOfWork unitOfWork, HmcrCurrentUser currentUser)
        {
            _userRepo = userRepo;
            _partyRepo = partyRepo;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
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

        public async Task<decimal> CreateUserAsync(UserCreateDto user)
        {
            //validation
            //username must be unique
            //field validation

            var userEntity = await _userRepo.CreateUserAsync(user);
            await _unitOfWork.CommitAsync();

            return userEntity.SystemUserId;
        }
    }
}
