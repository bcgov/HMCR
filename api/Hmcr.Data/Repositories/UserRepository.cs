using AutoMapper;
using Hmcr.Data.Database;
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
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IUserRepository : IHmcrRepositoryBase<HmrSystemUser>
    {
        Task<UserCurrentDto> GetCurrentUserAsync();
        Task<HmrSystemUser> GetCurrentActiveUserEntityAsync();
        Task<PagedDto<UserSearchDto>> GetUsersAsync(decimal[]? serviceAreas, string[]? userTypes, string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy);
        Task<UserDto> GetUserAsync(decimal systemUserId);
        Task<HmrSystemUser> CreateUserAsync(UserCreateDto user);
        Task<bool> DoesUsernameExistAsync(string username);
        Task UpdateUserAsync(UserUpdateDto userDto);
    }

    public class UserRepository : HmcrRepositoryBase<HmrSystemUser>, IUserRepository
    {
        private HmcrCurrentUser _currentUser;

        public UserRepository(AppDbContext dbContext, IMapper mapper, HmcrCurrentUser currentUser)
            : base(dbContext, mapper)
        {
            _currentUser = currentUser;
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
                                .FirstAsync(u => u.Username == _currentUser.UniversalId);

            var currentUser = Mapper.Map<UserCurrentDto>(userEntity);

            var permissions =
                userEntity
                .HmrUserRoles
                .Select(r => r.Role)
                .Where(r => r.EndDate == null || r.EndDate >= DateTime.Today) //active roles
                .SelectMany(r => r.HmrRolePermissions.Select(rp => rp.Permission))
                .Where(p => p.EndDate == null || p.EndDate >= DateTime.Today) //active permissions
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

            return currentUser;
        }

        public async Task<HmrSystemUser> GetCurrentActiveUserEntityAsync()
        {
            return await DbSet.FirstOrDefaultAsync(u => u.Username == _currentUser.UniversalId && (u.EndDate == null || u.EndDate >= DateTime.Today));
        }

        public async Task<PagedDto<UserSearchDto>> GetUsersAsync(decimal[]? serviceAreas, string[]? userTypes, string searchText, bool? isActive, int pageSize, int pageNumber, string orderBy)
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
                query = query
                    .Where(u => u.Username.Contains(searchText) || u.FirstName.Contains(searchText) || u.LastName.Contains(searchText) || u.BusinessLegalName.Contains(searchText));
            }
            
            if (isActive != null)
            {
                query = (bool)isActive
                    ? query.Where(u => u.EndDate == null || u.EndDate >= DateTime.Today)
                    : query.Where(u => u.EndDate != null || u.EndDate < DateTime.Today.AddDays(1));
            }

            query = query.Include(u => u.HmrServiceAreaUsers);

            var pagedEntity = await Page<HmrSystemUser, HmrSystemUser>(query, pageSize, pageNumber, orderBy);

            var users = Mapper.Map<IEnumerable<UserSearchDto>>(pagedEntity.SourceList);

            var userServiceArea = pagedEntity.SourceList.SelectMany(u => u.HmrServiceAreaUsers).ToLookup(u => u.SystemUserId);

            foreach (var user in users)
            {
                user.ServiceAreas = string.Join(",", userServiceArea[user.SystemUserId].Select(x => x.ServiceAreaNumber.ToString()));
                user.HasLogInHistory = pagedEntity.SourceList.Any(u => u.SystemUserId == user.SystemUserId && u.UserGuid != null);
            }

            var pagedDTO = new PagedDto<UserSearchDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = pagedEntity.TotalCount,
                SourceList = users
            };

            return pagedDTO;
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

            user.HasLogInHistory = userEntity.UserGuid != null;

            var roleIds =
                userEntity
                .HmrUserRoles
                .Where(r => r.EndDate == null || r.EndDate >= DateTime.Today) //active roles
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

        public async Task<bool> DoesUsernameExistAsync(string username)
        {
            return await DbSet.AnyAsync(u => u.Username == username);
        }

        public async Task<HmrSystemUser> CreateUserAsync(UserCreateDto user)
        {
            var userEntity = await AddAsync(user);

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

            return userEntity;
        }

        public async Task UpdateUserAsync(UserUpdateDto userDto)
        {
            var userEntity = await DbSet
                    .Include(x => x.HmrUserRoles)
                    .Include(x => x.HmrServiceAreaUsers)
                    .FirstAsync(u => u.SystemUserId == userDto.SystemUserId);

            Mapper.Map(userDto, userEntity);

            if (userEntity != null)
            {
                var tempUser = Mapper.Map<HmrSystemUser>(userDto);
            }
            else
            {
                throw new HmcrException($"User [{userDto.Username}] does not exist.");
            }

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
    }
}
