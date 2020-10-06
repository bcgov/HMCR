using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.ActivityCode;
using Hmcr.Model.Dtos.ActivityRule;
using Hmcr.Model.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IActivityCodeService
    {
        Task<PagedDto<ActivityCodeSearchDto>> GetActivityCodesAsync(string[]? maintenanceTypes, decimal[]? locationCodes, bool? isActive, string searchText, int pageSize, int pageNumber, string orderBy, string direction);
        Task<ActivityCodeSearchDto> GetActivityCodeAsync(decimal id);
        Task<IEnumerable<ActivityCodeLiteDto>> GetActivityCodesLiteAsync();
        Task<(bool NotFound, Dictionary<string, List<string>> Errors)> UpdateActivityCodeAsync(ActivityCodeUpdateDto activityCode);
        Task<(decimal id, Dictionary<string, List<string>> Errors)> CreateActivityCodeAsync(ActivityCodeCreateDto activityCode);
        Task<(bool NotFound, Dictionary<string, List<string>> Errors)> DeleteActivityCodeAsync(decimal id);
    }

    public class ActivityCodeService : IActivityCodeService
    {
        private IActivityCodeRepository _activityCodeRepo;
        private IFieldValidatorService _validatorService;
        private IUnitOfWork _unitOfWork;
        private IWorkReportRepository _workReportRepo;
        private ILocationCodeRepository _locationCodeRepo;
        private IActivityRuleRepository _activityRuleRepo;

        public ActivityCodeService(IActivityCodeRepository activityCodeRepo, IFieldValidatorService validatorService, IUnitOfWork unitOfWork,
            IWorkReportRepository workReportRepo, ILocationCodeRepository locationCodeRepo, IActivityRuleRepository activityRuleRepo)
        {
            _activityCodeRepo = activityCodeRepo;
            _validatorService = validatorService;
            _unitOfWork = unitOfWork;
            _workReportRepo = workReportRepo;
            _locationCodeRepo = locationCodeRepo;
            _activityRuleRepo = activityRuleRepo;
        }

        public async Task<(decimal id, Dictionary<string, List<string>> Errors)> CreateActivityCodeAsync(ActivityCodeCreateDto activityCode)
        {
            var errors = new Dictionary<string, List<string>>();

            if (await _activityCodeRepo.DoesActivityNumberExistAsync(activityCode.ActivityNumber))
            {
                errors.AddItem(Fields.ActivityNumber, $"ActivityNumber [{activityCode.ActivityNumber})] already exists.");
            }

            if (await _locationCodeRepo.DoesExistAsync(activityCode.LocationCodeId) == false)
            {
                errors.AddItem(Fields.LocationCodeId, $"LocationCodeId [{activityCode.LocationCodeId}] does not exist.");
            }

            var newLocationCode = (await _locationCodeRepo.GetLocationCode(activityCode.LocationCodeId)).LocationCode;
            
            var entityName = GetEntityName(newLocationCode);

            _validatorService.Validate(entityName, activityCode, errors);

            // location code is C validate FeatureType
            // location code is A or B, FeatureType is forced to null and SiteNumRequired is forced to false
            if (newLocationCode != "C")
            {
                IEnumerable<ActivityRuleDto> activityRuleDefaults = await _activityRuleRepo.GetDefaultRules();
                
                activityCode.FeatureType = null;
                activityCode.SpThresholdLevel = null;
                activityCode.IsSiteNumRequired = false;
                foreach (ActivityRuleDto activityRule in activityRuleDefaults)
                {
                    if (activityRule.ActivityRuleSet == "ROAD_LENGTH")
                    {
                        activityCode.RoadLengthRule = activityRule.ActivityRuleId;
                    }
                    else if (activityRule.ActivityRuleSet == "ROAD_CLASS")
                    {
                        activityCode.RoadClassRule = activityRule.ActivityRuleId;
                    }
                    else if (activityRule.ActivityRuleSet == "SURFACE_TYPE")
                    {
                        activityCode.SurfaceTypeRule = activityRule.ActivityRuleId;
                    }
                }
            }

            if (errors.Count > 0)
            {
                return (0, errors);
            }

            var activityCodeEntity = await _activityCodeRepo.CreateActivityCodeAsync(activityCode);
            _unitOfWork.Commit();

            return (activityCodeEntity.ActivityCodeId, errors);
        }

        public async Task<(bool NotFound, Dictionary<string, List<string>> Errors)> DeleteActivityCodeAsync(decimal id)
        {
            var errors = new Dictionary<string, List<string>>();

            var activityFromDB = await GetActivityCodeAsync(id);

            if (activityFromDB == null)
            {
                return (true, null);
            }

            if (await _workReportRepo.IsActivityNumberInUseAsync(activityFromDB.ActivityNumber))
            {
                errors.AddItem(Fields.ActivityNumber, $"ActivityNumber [{activityFromDB.ActivityNumber}] is in use and cannot be deleted.");
            }

            if (errors.Count > 0)
            {
                return (false, errors);
            }

            await _activityCodeRepo.DeleteActivityCodeAsync(id);
            _unitOfWork.Commit();

            return (false, errors);
        }

        public async Task<ActivityCodeSearchDto> GetActivityCodeAsync(decimal id)
        {
            return await _activityCodeRepo.GetActivityCodeAsync(id);
        }

        public async Task<PagedDto<ActivityCodeSearchDto>> GetActivityCodesAsync(string[]? maintenanceTypes, decimal[]? locationCodes, bool? isActive, string searchText, int pageSize, int pageNumber, string orderBy, string direction)
        {
            return await _activityCodeRepo.GetActivityCodesAsync(maintenanceTypes, locationCodes, isActive, searchText, pageSize, pageNumber, orderBy, direction);
        }

        public async Task<(bool NotFound, Dictionary<string, List<string>> Errors)> UpdateActivityCodeAsync(ActivityCodeUpdateDto activityCode)
        {
            var activityCodeFromDb = await GetActivityCodeAsync(activityCode.ActivityCodeId);
            
            if (activityCodeFromDb == null)
            {
                return (true, null);
            }

            var originalLocationCode = (await _locationCodeRepo.GetLocationCode(activityCodeFromDb.LocationCodeId)).LocationCode;

            var errors = new Dictionary<string, List<string>>();

            if (await _locationCodeRepo.DoesExistAsync(activityCode.LocationCodeId) == false)
            {
                errors.AddItem(Fields.LocationCodeId, $"LocationCodeId [{activityCode.LocationCodeId}] does not exist.");
            }

            //location codes can only be downgraded; C => B or A, B => A
            var newLocationCode = (await _locationCodeRepo.GetLocationCode(activityCode.LocationCodeId)).LocationCode;
            if (originalLocationCode == "A" && newLocationCode != "A")
            {
                errors.AddItem(Fields.LocationCodeId, $"LocationCode cannot be changed from A");
            }
            if (originalLocationCode == "B" && newLocationCode == "C")
            {
                errors.AddItem(Fields.LocationCodeId, $"LocationCode can only be changed to A");
            }
            
            var entityName = GetEntityName(newLocationCode);

            _validatorService.Validate(entityName, activityCode, errors);

            // location code is C validate FeatureType
            // location code is A or B, FeatureType is forced to null and SiteNumRequired is forced to false
            if (newLocationCode != "C")
            {
                IEnumerable<ActivityRuleDto> activityRuleDefaults = await _activityRuleRepo.GetDefaultRules();

                activityCode.FeatureType = null;
                activityCode.SpThresholdLevel = null;
                activityCode.IsSiteNumRequired = false;
                foreach (ActivityRuleDto activityRule in activityRuleDefaults)
                {
                    if (activityRule.ActivityRuleSet == "ROAD_LENGTH")
                    {
                        activityCode.RoadLengthRule = activityRule.ActivityRuleId;
                    }
                    else if (activityRule.ActivityRuleSet == "ROAD_CLASS")
                    {
                        activityCode.RoadClassRule = activityRule.ActivityRuleId;
                    }
                    else if (activityRule.ActivityRuleSet == "SURFACE_TYPE")
                    {
                        activityCode.SurfaceTypeRule = activityRule.ActivityRuleId;
                    }
                }
            }

            if (errors.Count > 0)
            {
                return (false, errors);
            }

            await _activityCodeRepo.UpdateActivityCodeAsync(activityCode);
            _unitOfWork.Commit();

            return (false, errors);
        }

        private string GetEntityName(string locationCode)
        {
            return locationCode == "C" ? Entities.ActivityCodeLocationCodeC : Entities.ActivityCode;
        }

        public async Task<IEnumerable<ActivityCodeLiteDto>> GetActivityCodesLiteAsync()
        {
            return await _activityCodeRepo.GetActiveActivityCodesLiteAsync();
        }
    }
}
