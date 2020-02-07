using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.ActivityCode;
using Hmcr.Model.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IActivityCodeService
    {
        Task<PagedDto<ActivityCodeSearchDto>> GetActivityCodesAsync(string[]? maintenanceTypes, decimal[]? locationCodes, bool? isActive, string searchText, int pageSize, int pageNumber, string orderBy, string direction);
        Task<ActivityCodeSearchDto> GetActivityCodeAsync(decimal id);
        Task<(bool NotFound, Dictionary<string, List<string>> Errors)> UpdateActivityCodeAsync(ActivityCodeUpdateDto activityCode);
        Task<(decimal id, Dictionary<string, List<string>> Errors)> CreateActivityCodeAsync(ActivityCodeCreateDto activityCode);
    }

    public class ActivityCodeService : IActivityCodeService
    {
        private IActivityCodeRepository _activityCodeRepo;
        private IFieldValidatorService _validatorService;
        private IUnitOfWork _unitOfWork;

        public ActivityCodeService(IActivityCodeRepository activityCodeRepo, IFieldValidatorService validatorService, IUnitOfWork unitOfWork)
        {
            _activityCodeRepo = activityCodeRepo;
            _validatorService = validatorService;
            _unitOfWork = unitOfWork;
        }

        public async Task<(decimal id, Dictionary<string, List<string>> Errors)> CreateActivityCodeAsync(ActivityCodeCreateDto activityCode)
        {
            var errors = new Dictionary<string, List<string>>();
            /* TODO: validation required?? see HMCR-234
             * - Need a new ActivityCode Entities Const 
             * - Rules added to FieldValidationRules
            var entityName = Entities.ActivityCode;
            var errors = new Dictionary<string, List<string>>();

            _validatorService.Validate(entityName, activityCode, errors);
            */

            /* TODO: Check for ActivityNumber existence */
            if (await _activityCodeRepo.DoesActivityNumberExistAsync(activityCode.ActivityNumber))
            {
                errors.AddItem(Fields.ActivityNumber, $"ActivityNumber [{activityCode.ActivityNumber})] already exists.");
            }

            if (errors.Count > 0)
            {
                return (0, errors);
            }

            var activityCodeEntity = await _activityCodeRepo.CreateActivityCodeAsync(activityCode);
            await _unitOfWork.CommitAsync();

            return (activityCodeEntity.ActivityCodeId, errors);
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

            var errors = new Dictionary<string, List<string>>();
            /* TODO: validation required?? see HMCR-236 
             * - Field rules added to FieldValidationRules?
             * - Point-Line rules
             * - Location Code rules
             * - Disable/Delete rules
            var entityName = Entities.ActivityCode;
            var errors = new Dictionary<string, List<string>>();

            _validatorService.Validate(entityName, activityCode, errors);
            */

            if (errors.Count > 0)
            {
                return (false, errors);
            }

            await _activityCodeRepo.UpdateActivityCodeAsync(activityCode);
            await _unitOfWork.CommitAsync();

            return (false, errors);
        }
    }
}
