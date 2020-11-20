using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IUserRepository : IHmcrRepositoryBase<HmrSystemUser>
    {
        Task<UserCurrentDto> GetCurrentUserAsync();
        Task<PagedDto<UserSearchDto>> GetUsersAsync(decimal[]? serviceAreas, string[]? userTypes, string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy, string direction);
        Task<UserDto> GetUserAsync(decimal systemUserId);
        Task<HmrSystemUser> CreateUserAsync(UserCreateDto user, BceidAccount account);
        Task<bool> DoesUsernameExistAsync(string username, string userType);
        Task UpdateUserAsync(UserUpdateDto userDto);
        Task DeleteUserAsync(UserDeleteDto user);
        Task<HmrSystemUser> GetActiveUserEntityAsync(Guid userGuid);
        Task UpdateUserFromBceidAsync(BceidAccount user, long concurrencyControlNumber);
        IEnumerable<UserDto> GetActiveUsersByServiceAreaNumber(decimal serviceAreaNumber);
        Task UpdateUserApiClientId(string clientId);
        Task<IEnumerable<UserSearchExportDto>> GetUsersByFilterAsync(decimal[]? serviceAreas, string[]? userTypes, string searchText, bool? isActive);
    }

    public class UserRepository : HmcrRepositoryBase<HmrSystemUser>, IUserRepository
    {
        private HmcrCurrentUser _currentUser;
        private IPartyRepository _partyRepo;

        public UserRepository(AppDbContext dbContext, IMapper mapper, HmcrCurrentUser currentUser, IPartyRepository partyRepo)
            : base(dbContext, mapper)
        {
            _currentUser = currentUser;
            _partyRepo = partyRepo;
        }

        public async Task<UserCurrentDto> GetCurrentUserAsync()
        {
            var userEntity = await DbSet.AsNoTracking()
                                .Include(x => x.HmrUserRoles)
                                    .ThenInclude(x => x.Role)
                                        .ThenInclude(x => x.HmrRolePermissions)
                                            .ThenInclude(x => x.Permission)
                                .Include(x => x.HmrServiceAreaUsers)
                                    .ThenInclude(x => x.ServiceAreaNumberNavigation)
                                .FirstAsync(u => u.UserGuid == _currentUser.UserGuid);

            var currentUser = Mapper.Map<UserCurrentDto>(userEntity);

            var permissions =
                userEntity
                .HmrUserRoles
                .Select(r => r.Role)
                .Where(r => r.EndDate == null || r.EndDate > DateTime.Today) //active roles
                .SelectMany(r => r.HmrRolePermissions.Select(rp => rp.Permission))
                .Where(p => p.EndDate == null || p.EndDate > DateTime.Today) //active permissions
                .ToLookup(p => p.Name)
                .Select(p => p.First())
                .Select(p => p.Name)
                .ToList();

            currentUser.Permissions = permissions;

            var serviceAreas =
                userEntity
                .HmrServiceAreaUsers
                .Select(s => s.ServiceAreaNumberNavigation);

            currentUser.ServiceAreas = new List<ServiceAreaDto>(Mapper.Map<IEnumerable<ServiceAreaDto>>(serviceAreas));

            currentUser.IsSystemAdmin = userEntity.HmrUserRoles.Any(x => x.Role.Name == Constants.SystemAdmin);

            return currentUser;
        }

        public async Task<HmrSystemUser> GetActiveUserEntityAsync(Guid userGuid)
        {
            return await DbSet.Include(x => x.Party).FirstOrDefaultAsync(u => u.UserGuid == userGuid && (u.EndDate == null || u.EndDate > DateTime.Today));
        }

        /// <summary>
        /// This method can be called concurrently by a typical client which asynchronously calls APIs
        /// In order to avoid unecessary multiple updates and the concurrency control number exception,
        /// it runs SQL with optimistic concurrency control.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="concurrencyControlNumber"></param>
        /// <returns></returns>
        public async Task UpdateUserFromBceidAsync(BceidAccount user, long concurrencyControlNumber)
        {
            var sql = new StringBuilder("UPDATE HMR_SYSTEM_USER SET ");
            sql.Append("USERNAME = {0}, ");
            sql.Append("FIRST_NAME = {1}, ");
            sql.Append("LAST_NAME = {2}, ");
            sql.Append("EMAIL = {3}, ");
            sql.Append("CONCURRENCY_CONTROL_NUMBER = CONCURRENCY_CONTROL_NUMBER + 1 ");
            sql.Append("WHERE USER_GUID = {4} AND CONCURRENCY_CONTROL_NUMBER = {5} ");

            await DbContext.Database.ExecuteSqlRawAsync(sql.ToString(), user.Username, user.FirstName, user.LastName, user.Email, user.UserGuid, concurrencyControlNumber);
        }

        public async Task<PagedDto<UserSearchDto>> GetUsersAsync(decimal[]? serviceAreas, string[]? userTypes, string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy, string direction)
        {
            var query = DbSet.AsNoTracking();

            if (serviceAreas != null && serviceAreas.Length > 0)
            {
                query = query.Where(u => u.HmrServiceAreaUsers.Any(s => serviceAreas.Contains(s.ServiceAreaNumber)));
            }

            if (userTypes != null && userTypes.Length > 0)
            {
                query = query.Where(u => userTypes.Contains(u.UserType));
            }

            if (searchText.IsNotEmpty())
            {
                searchText = searchText.Trim();

                query = query
                    .Where(u => u.Username.Contains(searchText) || u.FirstName.Contains(searchText) || (u.FirstName + " " + u.LastName).Contains(searchText) || u.LastName.Contains(searchText) || u.BusinessLegalName.Contains(searchText));
            }

            if (isActive != null)
            {
                query = (bool)isActive
                    ? query.Where(u => u.EndDate == null || u.EndDate > DateTime.Today)
                    : query.Where(u => u.EndDate != null && u.EndDate <= DateTime.Today);
            }

            query = query.Include(u => u.HmrServiceAreaUsers);

            var pagedEntity = await Page<HmrSystemUser, HmrSystemUser>(query, pageSize, pageNumber, orderBy, direction);

            var users = Mapper.Map<IEnumerable<UserSearchDto>>(pagedEntity.SourceList);

            var userServiceArea = pagedEntity.SourceList.SelectMany(u => u.HmrServiceAreaUsers).ToLookup(u => u.SystemUserId);

            foreach (var user in users)
            {
                user.ServiceAreas = string.Join(",", userServiceArea[user.SystemUserId].Select(x => x.ServiceAreaNumber).OrderBy(x => x));
                user.HasLogInHistory = pagedEntity.SourceList.Any(u => u.SystemUserId == user.SystemUserId && u.UserGuid != null);
            }

            var pagedDTO = new PagedDto<UserSearchDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = pagedEntity.TotalCount,
                SourceList = users,
                OrderBy = orderBy,
                Direction = direction
            };

            return pagedDTO;
        }
        public async Task<IEnumerable<UserSearchExportDto>> GetUsersByFilterAsync(decimal[]? serviceAreas, string[]? userTypes, string searchText, bool? isActive)
        {
            var query = DbSet.AsNoTracking();

            if (serviceAreas != null && serviceAreas.Length > 0)
            {
                query = query.Where(u => u.HmrServiceAreaUsers.Any(s => serviceAreas.Contains(s.ServiceAreaNumber)));
            }

            if (userTypes != null && userTypes.Length > 0)
            {
                query = query.Where(u => userTypes.Contains(u.UserType));
            }

            if (searchText.IsNotEmpty())
            {
                searchText = searchText.Trim();

                query = query
                    .Where(u => u.Username.Contains(searchText) 
                    || u.FirstName.Contains(searchText) 
                    || (u.FirstName + " " + u.LastName).Contains(searchText) 
                    || u.LastName.Contains(searchText) 
                    || u.BusinessLegalName.Contains(searchText));
            }

            if (isActive != null)
            {
                query = (bool)isActive
                    ? query.Where(u => u.EndDate == null || u.EndDate > DateTime.Today)
                    : query.Where(u => u.EndDate != null && u.EndDate <= DateTime.Today);
            }

            var entityUsers = await query
                .Include(u => u.HmrServiceAreaUsers)
                .Include(u => u.HmrUserRoles)
                .ToListAsync();

            var users = Mapper.Map<IEnumerable<UserSearchExportDto>>(entityUsers);

            var userServiceArea = entityUsers.SelectMany(u => u.HmrServiceAreaUsers).ToLookup(u => u.SystemUserId);
            int totalServiceArea = await DbContext.HmrServiceAreas.AsNoTracking().CountAsync();
            var roles = await DbContext.HmrRoles.AsNoTracking().ToListAsync();
            var userRoles = entityUsers
                            .Where(r => r.EndDate == null || r.EndDate > DateTime.Today)
                            .SelectMany(u => u.HmrUserRoles)
                            .ToLookup(u => u.SystemUserId);

            foreach (var user in users)
            {
                string userServiceAreas = string.Join(",", userServiceArea[user.SystemUserId].Select(x => x.ServiceAreaNumber).OrderBy(x => x));
                if (userServiceAreas.Count(f => f == ',') >0 )
                {
                    if (userServiceAreas.Count(f => f == ',') + 1 == totalServiceArea)
                    {
                        user.ServiceAreas = "ALL";
                    }
                    else
                    {
                        user.ServiceAreas = string.Format("\"{0}\"", userServiceAreas);
                    }
                }
                else
                {
                    user.ServiceAreas = userServiceAreas;
                }
                var roleIds = userRoles[user.SystemUserId].Select(x => x.RoleId).Distinct();
                string uRoles = string.Join(",", roles.Where(x => roleIds.Contains(x.RoleId)).Select(y => y.Name).OrderBy(z => z));
                if (uRoles.Count(f => f == ',') > 0)
                {
                    user.UserRoles = string.Format("\"{0}\"", uRoles);
                }
                else
                {
                    user.UserRoles = uRoles;
                }
                user.HasLogInHistory = entityUsers.Any(u => u.SystemUserId == user.SystemUserId && u.UserGuid != null);
            }

            

            return users;
        }

        public async Task<UserDto> GetUserAsync(decimal systemUserId)
        {
            var userEntity = await DbSet.AsNoTracking()
                    .Include(x => x.HmrUserRoles)
                    .Include(x => x.HmrServiceAreaUsers)
                    .FirstOrDefaultAsync(u => u.SystemUserId == systemUserId);

            if (userEntity == null)
                return null;

            var user = Mapper.Map<UserDto>(userEntity);

            var roleIds =
                userEntity
                .HmrUserRoles
                .Where(r => r.EndDate == null || r.EndDate > DateTime.Today) //active roles
                .Select(r => r.RoleId)
                .ToList();

            user.UserRoleIds = roleIds;

            var serviceAreasNumbers =
                userEntity
                .HmrServiceAreaUsers
                .Select(s => s.ServiceAreaNumber)
                .ToList();

            user.ServiceAreaNumbers = serviceAreasNumbers;

            return user;
        }

        public async Task<bool> DoesUsernameExistAsync(string username, string userType)
        {
            return await DbSet.AnyAsync(u => u.Username == username && u.UserType == userType);
        }

        public async Task<HmrSystemUser> CreateUserAsync(UserCreateDto user, BceidAccount account)
        {
            var userEntity = new HmrSystemUser
            {
                Username = account.Username.ToUpperInvariant(),
                UserGuid = account.UserGuid,
                BusinessGuid = account.BusinessGuid,
                BusinessLegalName = account.BusinessLegalName,
                UserType = account.UserType,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email,
                EndDate = user.EndDate,
                UserDirectory = user.UserDirectory
            };

            if (account.UserType != UserTypeDto.INTERNAL)
            {
                var partyEntity = _partyRepo.GetPartyEntityByGuid(account.BusinessGuid);

                if (partyEntity == null)
                {
                    userEntity.Party = new HmrParty
                    {
                        BusinessGuid = account.BusinessGuid,
                        BusinessLegalName = account.BusinessLegalName?.Trim(),
                        BusinessNumber = account.BusinessNumber,
                        DisplayName = account.DisplayName?.Trim()
                    };
                }
                else
                {
                    userEntity.PartyId = partyEntity.PartyId;
                }
            }

            foreach (var areaNumber in user.ServiceAreaNumbers)
            {
                userEntity.HmrServiceAreaUsers
                    .Add(new HmrServiceAreaUser
                    {
                        ServiceAreaNumber = areaNumber
                    });
            }

            foreach (var roleId in user.UserRoleIds)
            {
                userEntity.HmrUserRoles
                    .Add(new HmrUserRole
                    {
                        RoleId = roleId
                    }); ;
            }

            await DbSet.AddAsync(userEntity);

            return userEntity;
        }

        public async Task UpdateUserAsync(UserUpdateDto userDto)
        {
            //remove time portion
            userDto.EndDate = userDto.EndDate?.Date;

            var userEntity = await DbSet
                    .Include(x => x.HmrUserRoles)
                    .Include(x => x.HmrServiceAreaUsers)
                    .FirstAsync(u => u.SystemUserId == userDto.SystemUserId);

            Mapper.Map(userDto, userEntity);

            SyncRoles(userDto, userEntity);

            SyncServiceAreas(userDto, userEntity);
        }

        private void SyncRoles(UserUpdateDto userDto, HmrSystemUser userEntity)
        {
            var rolesToDelete =
                userEntity.HmrUserRoles.Where(r => !userDto.UserRoleIds.Contains(r.RoleId)).ToList();

            for (var i = rolesToDelete.Count() - 1; i >= 0; i--)
            {
                DbContext.Remove(rolesToDelete[i]);
            }

            var existingRoleIds = userEntity.HmrUserRoles.Select(r => r.RoleId);

            var newRoleIds = userDto.UserRoleIds.Where(r => !existingRoleIds.Contains(r));

            foreach (var roleId in newRoleIds)
            {
                userEntity.HmrUserRoles
                    .Add(new HmrUserRole
                    {
                        RoleId = roleId,
                        SystemUserId = userEntity.SystemUserId
                    });
            }
        }

        private void SyncServiceAreas(UserUpdateDto userDto, HmrSystemUser userEntity)
        {
            var areasToDelete =
                userEntity.HmrServiceAreaUsers.Where(s => !userDto.ServiceAreaNumbers.Contains(s.ServiceAreaNumber)).ToList();

            for (var i = areasToDelete.Count() - 1; i >= 0; i--)
            {
                DbContext.Remove(areasToDelete[i]);
            }

            var existingAreaNumbers = userEntity.HmrServiceAreaUsers.Select(s => s.ServiceAreaNumber);

            var newAreaNumbers = userDto.ServiceAreaNumbers.Where(r => !existingAreaNumbers.Contains(r));

            foreach (var areaNumber in newAreaNumbers)
            {
                userEntity.HmrServiceAreaUsers
                    .Add(new HmrServiceAreaUser
                    {
                        ServiceAreaNumber = areaNumber,
                        SystemUserId = userEntity.SystemUserId
                    });
            }
        }

        public async Task DeleteUserAsync(UserDeleteDto user)
        {
            //remove time portion
            user.EndDate = user.EndDate?.Date;

            var userEntity = await DbSet
                .FirstAsync(u => u.SystemUserId == user.SystemUserId);

            Mapper.Map(user, userEntity);
        }


        public IEnumerable<UserDto> GetActiveUsersByServiceAreaNumber(decimal serviceAreaNumber)
        {
            return GetAll<UserDto>(x => (x.EndDate == null || x.EndDate > DateTime.Today) && x.HmrServiceAreaUsers.Any(y => y.ServiceAreaNumber == serviceAreaNumber));
        }

        public async Task UpdateUserApiClientId(string apiClientId)
        {
            var userEntity = await DbSet.FirstAsync(u => u.UserGuid == _currentUser.UserGuid);

            userEntity.ApiClientId = apiClientId;
        }
    }
}
