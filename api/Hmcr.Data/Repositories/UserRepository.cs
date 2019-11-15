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
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IUserRepository : IHmcrRepositoryBase<HmrSystemUser>
    {
        Task<UserCurrentDto> GetCurrentUser();
    }

    public class UserRepository : HmcrRepositoryBase<HmrSystemUser>, IUserRepository
    {
        private SmHeaders _smHeaders;

        public UserRepository(AppDbContext dbContext, IMapper mapper, SmHeaders smHeaders)
            : base(dbContext, mapper)
        {
            _smHeaders = smHeaders;
        }

        public async Task<UserCurrentDto> GetCurrentUser()
        {
            var userEntity = await DbSet.AsNoTracking()
                                .Include(x => x.HmrUserRoles)
                                    .ThenInclude(x => x.Role)
                                        .ThenInclude(x => x.HmrRolePermissions)
                                            .ThenInclude(x => x.Permission)
                                .Include(x => x.HmrServiceAreaUsers)
                                    .ThenInclude(x => x.ServiceAreaNumberNavigation)
                                .FirstAsync(u => u.Username == _smHeaders.UniversalId);

            var currentUser = Mapper.Map<UserCurrentDto>(userEntity);

            var permissions =
                userEntity
                .HmrUserRoles
                .Select(r => r.Role)
                .SelectMany(r => r.HmrRolePermissions.Select(rp => rp.Permission))
                .Where(p => p.EndDate == null || p.EndDate >= DateTime.Today)
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
    }
}
