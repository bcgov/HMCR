using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.Role;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IRoleRepository : IHmcrRepositoryBase<HmrRole>
    {
        Task<IEnumerable<RoleDto>> GetActiveRolesAsync();
        Task<int> CountActiveRoleIdsAsync(IEnumerable<decimal> roles);
    }
    public class RoleRepository : HmcrRepositoryBase<HmrRole>, IRoleRepository
    {
        public RoleRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<int> CountActiveRoleIdsAsync(IEnumerable<decimal> roles)
        {
            return await DbSet.CountAsync(r => roles.Contains(r.RoleId) && (r.EndDate == null || r.EndDate >= DateTime.Today));
        }

        public async Task<IEnumerable<RoleDto>> GetActiveRolesAsync()
        {
            var roleEntity = await DbSet.AsNoTracking()
                .Where(x => x.EndDate == null || x.EndDate >= DateTime.Today)
                .ToListAsync();

            return Mapper.Map<IEnumerable<RoleDto>>(roleEntity);
        } 
    }
}
