using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model;
using Hmcr.Model.Dtos.Permission;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IUserRepository : IHmcrRepositoryBase<HmrSystemUser>
    {
        Task<UserCurrentDto> GetCurrentUserAsync();
        Task<HmrSystemUser> GetCurrentActiveUserEntityAsync();
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
    }
}
