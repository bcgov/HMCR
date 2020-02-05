using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos;
using Hmcr.Model.Utils;
using Hmcr.Model.Dtos.ActivityCode;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface IActivityCodeRepository
    {
        Task<IEnumerable<ActivityCodeDto>> GetActiveActivityCodesAsync();
        Task<PagedDto<ActivityCodeSearchDto>> GetActivityCodesAsync(string[]? maintenanceTypes, decimal[]? locationCodes, bool? isActive, string searchText, int pageSize, int pageNumber, string orderBy, string direction);
        Task<ActivityCodeSearchDto> GetActivityCodeAsync(decimal id);
        Task<(bool NotFound, Dictionary<string, List<string>> Errors)> UpdateActivityCodeAsync(ActivityCodeUpdateDto activityCode);
        Task<(decimal id, Dictionary<string, List<string>> Errors)> CreateActivityCodeAsync(ActivityCodeCreateDto activityCode);
    }

    public class ActivityCodeRepository : HmcrRepositoryBase<HmrActivityCode>, IActivityCodeRepository
    {
        public ActivityCodeRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<ActivityCodeDto>> GetActiveActivityCodesAsync()
        {
            var activities = await DbSet
                .Include(x => x.LocationCode)
                .ToListAsync();

            return Mapper.Map<IEnumerable<ActivityCodeDto>>(activities).Where(x => x.IsActive);
        }

        public Task<(decimal id, Dictionary<string, List<string>> Errors)> CreateActivityCodeAsync(ActivityCodeCreateDto activityCode)
        {
            throw new System.NotImplementedException();
        }
        
        public Task<ActivityCodeSearchDto> GetActivityCodeAsync(decimal id)
        {
            throw new System.NotImplementedException();
        }

        public Task<PagedDto<ActivityCodeSearchDto>> GetActivityCodesAsync(string[]? maintenanceTypes, decimal[]? locationCodes, bool? isActive, string searchText, int pageSize, int pageNumber, string orderBy, string direction)
        {
            var query = DbSet.AsNoTracking();
            
            if (maintenanceTypes != null && maintenanceTypes.Length > 0)
            {
                query = query.Where(ac => maintenanceTypes.Contains(ac.MaintenanceType));
            }

            if (locationCodes != null && locationCodes.Length > 0)
            {
                query = query.Where(ac => locationCodes.Contains(ac.LocationCodeId));
            }

            if (isActive != null)
            {
                query = (bool)isActive
                    ? query.Where(ac => ac.EndDate == null || ac.EndDate > DateTime.Today)
                    : query.Where(ac => ac.EndDate != null || ac.EndDate <= DateTime.Today.AddDays(1));
            }

            if (searchText.IsNotEmpty())
            {
                query = query
                    .Where(ac => ac.ActivityName.Contains(searchText) || ac.ActivityNumber.Contains(searchText));
            }

            throw new System.NotImplementedException();
        }

        public Task<(bool NotFound, Dictionary<string, List<string>> Errors)> UpdateActivityCodeAsync(ActivityCodeUpdateDto activityCode)
        {
            throw new System.NotImplementedException();
        }
    }
}
