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
        Task<IEnumerable<ActivityCodeLiteDto>> GetActiveActivityCodesLiteAsync();
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

        public async Task<IEnumerable<ActivityCodeLiteDto>> GetActiveActivityCodesLiteAsync()
        {
            var activities = await DbSet
                .OrderBy(x => x.ActivityNumber)
                .ThenBy(x => x.ActivityName)
                .Select(x => new ActivityCodeLiteDto()
                {
                    Id = x.ActivityCodeId,
                    Name = $"{x.ActivityNumber}-{x.ActivityName}",
                    ActivityNumber = x.ActivityNumber
                })
                .ToListAsync();

            return activities;
        }

        public async Task<HmrActivityCode> CreateActivityCodeAsync(ActivityCodeCreateDto activityCode)
        {
            var activityCodeEntity = new HmrActivityCode();

            Mapper.Map(activityCode, activityCodeEntity);

            
            foreach (var ruleId in activityCode.ActivityRuleIds)
            {
                activityCodeEntity.HmrActivityCodeRules
                    .Add(new HmrActivityCodeRule
                    {
                        ActivityRuleId = ruleId
                    });
            }

            //TODO: add in saving of Service Areas

            await DbSet.AddAsync(activityCodeEntity);

            return activityCodeEntity;
        }

        public async Task<ActivityCodeSearchDto> GetActivityCodeAsync(decimal id)
        {
            var activityCodeEntity = await DbSet.AsNoTracking()
                //todo ServiceArea
                //.Include(x => x.HmrServiceAreaRules) //new table
                .Include(x => x.HmrActivityCodeRules) //new table
                .FirstOrDefaultAsync(ac => ac.ActivityCodeId == id);

            if (activityCodeEntity == null)
                return null;

            var activityCode = Mapper.Map<ActivityCodeSearchDto>(activityCodeEntity);

            activityCode.IsReferenced = await _workReportRepo.IsActivityNumberInUseAsync(activityCode.ActivityNumber);
            
            var activityRules = activityCodeEntity
                .HmrActivityCodeRules
                .Select(s => s.ActivityCodeRuleId)
                .ToList();

            /*var activityRules = new List<decimal> { 1, 3 };
            activityCode.ActivityRuleIds = activityRules;
            activityCode.RoadLengthRule = 2;
            activityCode.SurfaceTypeRule = 3;
            activityCode.RoadClassRule = 1;*/

            //TODO: pull the service areas
            //var serviceAreasNumbers =
            //    activityCodeEntity
            //    .HmrServiceAreaRules //new table
            //    .Select(s => s.ServiceAreaNumber)
            //    .ToList();
            var serviceAreasNumbers = new List<decimal> { 1, 9 };
            activityCode.ServiceAreaNumbers = serviceAreasNumbers;

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

            SyncActivityRules(activityCode, activityCodeEntity);
            
            //TODO: call function to sync service area changes
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

        private void SyncActivityRules(ActivityCodeUpdateDto activityUpdateDto, HmrActivityCode activityCodeEntity)
        {
            var rulesToDelete =
                activityCodeEntity.HmrActivityCodeRules.Where(r => !activityUpdateDto.ActivityRuleIds.Contains(r.ActivityRuleId)).ToList();

            for (var i = rulesToDelete.Count() - 1; i >= 0; i--)
            {
                DbContext.Remove(rulesToDelete[i]);
            }

            var existingRuleIds = activityCodeEntity.HmrActivityCodeRules.Select(r => r.ActivityRuleId);

            var newRuleIds = activityUpdateDto.ActivityRuleIds.Where(r => !existingRuleIds.Contains(r));

            foreach (var ruleId in newRuleIds)
            {
                activityCodeEntity.HmrActivityCodeRules
                    .Add(new HmrActivityCodeRule
                    {
                        ActivityRuleId = ruleId
                    });
            }
        }
    }
}
