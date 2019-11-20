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
    }

    public class UserRepository : HmcrRepositoryBase<HmrSystemUser>, IUserRepository
    {
        private HmcrCurrentUser _currentUser;
        private IUnitOfWork _unitOfWork;

        public UserRepository(AppDbContext dbContext, IMapper mapper, HmcrCurrentUser currentUser, IUnitOfWork unitOfwork)
            : base(dbContext, mapper)
        {
            _currentUser = currentUser;
            _unitOfWork = unitOfwork;
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
            return Mapper.Map<UserDto>(await DbSet.FindAsync(systemUserId));
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
    }
}
