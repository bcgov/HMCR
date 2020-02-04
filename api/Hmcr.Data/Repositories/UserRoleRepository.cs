using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IUserRoleRepository : IHmcrRepositoryBase<HmrUserRole>
    {
        Task<bool> IsRoleInUseAsync(decimal roleId);
    }

    public class UserRoleRepository : HmcrRepositoryBase<HmrUserRole>, IUserRoleRepository
    {
        public UserRoleRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<bool> IsRoleInUseAsync(decimal roleId)
        {
            return await DbSet.AnyAsync(x => x.RoleId == roleId);
        }

    }
}
