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
    }
    public class RoleRepository : HmcrRepositoryBase<HmrRole>, IRoleRepository
    {
        public RoleRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
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
