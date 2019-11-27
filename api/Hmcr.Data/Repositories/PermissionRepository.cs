using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.Permission;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IPermissionRepository : IHmcrRepositoryBase<HmrPermission>
    {
        Task<IEnumerable<PermissionDto>> GetActivePermissionsAsync();
        Task<int> CountActivePermissionIdsAsnyc(IEnumerable<decimal> permissions);
    }
    public class PermissionRepository : HmcrRepositoryBase<HmrPermission>, IPermissionRepository
    {
        public PermissionRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }
        public async Task<int> CountActivePermissionIdsAsnyc(IEnumerable<decimal> permissions)
        {
            return await DbSet.CountAsync(x => permissions.Contains(x.PermissionId) && (x.EndDate == null || x.EndDate > DateTime.Today));
        }
        public async Task<IEnumerable<PermissionDto>> GetActivePermissionsAsync()
        {
            var permissionEntity = await DbSet.AsNoTracking()
                .Where(x => x.EndDate == null || x.EndDate > DateTime.Today)
                .ToListAsync();

            return Mapper.Map<IEnumerable<PermissionDto>>(permissionEntity);
        }

    }
}
