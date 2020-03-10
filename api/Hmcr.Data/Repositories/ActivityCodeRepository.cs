using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.ActivityCode;
using Hmcr.Model.Utils;
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
        Task<HmrActivityCode> CreateActivityCodeAsync(ActivityCodeCreateDto activityCode);
        Task UpdateActivityCodeAsync(ActivityCodeUpdateDto activityCode);
        Task<bool> DoesActivityNumberExistAsync(string activityNumber);
        Task DeleteActivityCodeAsync(decimal id);
    }

    public class ActivityCodeRepository : HmcrRepositoryBase<HmrActivityCode>, IActivityCodeRepository
    {
        private IWorkReportRepository _workReportRepo;

        public ActivityCodeRepository(AppDbContext dbContext, IMapper mapper, IWorkReportRepository workReportRepo)
            : base(dbContext, mapper)
        {
            _workReportRepo = workReportRepo;
        }

        public async Task<IEnumerable<ActivityCodeDto>> GetActiveActivityCodesAsync()
        {
            var activities = await DbSet
                .Include(x => x.LocationCode)
                .ToListAsync();

            return Mapper.Map<IEnumerable<ActivityCodeDto>>(activities).Where(x => x.IsActive);
        }

        public async Task<HmrActivityCode> CreateActivityCodeAsync(ActivityCodeCreateDto activityCode)
        {
            var activityCodeEntity = new HmrActivityCode();
            
            Mapper.Map(activityCode, activityCodeEntity);

            await DbSet.AddAsync(activityCodeEntity);
            
            return activityCodeEntity;
        }
        
        public async Task<ActivityCodeSearchDto> GetActivityCodeAsync(decimal id)
        {
            var activityCodeEntity = await DbSet.AsNoTracking()
                .FirstOrDefaultAsync(ac => ac.ActivityCodeId == id);

            if (activityCodeEntity == null)
                return null;

            var activityCode = Mapper.Map<ActivityCodeSearchDto>(activityCodeEntity);

            activityCode.IsReferenced = await _workReportRepo.IsActivityNumberInUseAsync(activityCode.ActivityNumber);

            return activityCode;
        }

        public async Task<PagedDto<ActivityCodeSearchDto>> GetActivityCodesAsync(string[]? maintenanceTypes, decimal[]? locationCodes, bool? isActive, string searchText, int pageSize, int pageNumber, string orderBy, string direction)
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
                    : query.Where(ac => ac.EndDate != null && ac.EndDate <= DateTime.Today);
            }

            if (searchText.IsNotEmpty())
            {
                query = query
                    .Where(ac => ac.ActivityName.Contains(searchText) || ac.ActivityNumber.Contains(searchText));
            }

            var pagedEntity = await Page<HmrActivityCode, HmrActivityCode>(query, pageSize, pageNumber, orderBy, direction);

            var activityCodes = Mapper.Map<IEnumerable<ActivityCodeSearchDto>>(pagedEntity.SourceList);

            // Find out which activity numbers are being used
            await foreach (var activityNumber in FindActivityNumbersInUseAync(activityCodes.Select(ac => ac.ActivityNumber)))
            {
                activityCodes.FirstOrDefault(ac => ac.ActivityNumber == activityNumber).IsReferenced = true;
            }

            var pagedDTO = new PagedDto<ActivityCodeSearchDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = pagedEntity.TotalCount,
                SourceList = activityCodes,
                OrderBy = orderBy,
                Direction = direction
            };

            return pagedDTO;
        }

        public async Task UpdateActivityCodeAsync(ActivityCodeUpdateDto activityCode)
        {
            var activityCodeEntity = await DbSet
                    .FirstAsync(ac => ac.ActivityCodeId == activityCode.ActivityCodeId);

            activityCode.EndDate = activityCode.EndDate?.Date;

            Mapper.Map(activityCode, activityCodeEntity);
        }

        public async Task DeleteActivityCodeAsync(decimal id)
        {
            var activityCodeEntity = await DbSet
                .FirstAsync(ac => ac.ActivityCodeId == id);

            DbSet.Remove(activityCodeEntity);
        }

        public async Task<bool> DoesActivityNumberExistAsync(string activityNumber)
        {
            return await DbSet.AnyAsync(ac => ac.ActivityNumber == activityNumber);
        }

        private async IAsyncEnumerable<string> FindActivityNumbersInUseAync(IEnumerable<string> activityNumbers)
        {
            foreach (var activityNumber in activityNumbers)
            {
                if (await _workReportRepo.IsActivityNumberInUseAsync(activityNumber))
                    yield return activityNumber;
            }
        }
    }
}
