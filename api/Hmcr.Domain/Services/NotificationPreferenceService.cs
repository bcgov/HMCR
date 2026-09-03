using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos.NotificationPreference;
using Hmcr.Model.Dtos.User;
using System;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public enum NotificationPreferenceUpdateResult
    {
        Updated,
        ServiceAreaNotAssigned,
        UnsupportedSubmissionStream
    }

    public interface INotificationPreferenceService
    {
        bool IsCurrentUserEligible { get; }
        Task<NotificationPreferencesDto> GetCurrentUserPreferencesAsync();
        Task<NotificationPreferenceUpdateResult> UpdateCurrentUserPreferenceAsync(
            decimal serviceAreaNumber,
            decimal submissionStreamId,
            NotificationPreferenceUpdateDto update);
        Task UpdateAllCurrentUserPreferencesAsync(bool enabled);
        Task InitializeDefaultsForInternalUserAsync(decimal systemUserId);
    }

    public class NotificationPreferenceService : INotificationPreferenceService
    {
        private readonly INotificationPreferenceRepository _notificationPreferenceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HmcrCurrentUser _currentUser;

        public NotificationPreferenceService(
            INotificationPreferenceRepository notificationPreferenceRepository,
            IUnitOfWork unitOfWork,
            HmcrCurrentUser currentUser)
        {
            _notificationPreferenceRepository = notificationPreferenceRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public bool IsCurrentUserEligible =>
            string.Equals(_currentUser.UserInfo?.UserType, UserTypeDto.INTERNAL, StringComparison.OrdinalIgnoreCase);

        public async Task<NotificationPreferencesDto> GetCurrentUserPreferencesAsync()
        {
            EnsureCurrentUserIsEligible();
            return await _notificationPreferenceRepository.GetForUserAsync(_currentUser.UserInfo.SystemUserId);
        }

        public async Task<NotificationPreferenceUpdateResult> UpdateCurrentUserPreferenceAsync(
            decimal serviceAreaNumber,
            decimal submissionStreamId,
            NotificationPreferenceUpdateDto update)
        {
            EnsureCurrentUserIsEligible();

            var serviceAreaUserId = await _notificationPreferenceRepository.GetActiveServiceAreaUserIdAsync(
                _currentUser.UserInfo.SystemUserId,
                serviceAreaNumber);

            if (!serviceAreaUserId.HasValue)
            {
                return NotificationPreferenceUpdateResult.ServiceAreaNotAssigned;
            }

            if (!await _notificationPreferenceRepository.IsSupportedSubmissionStreamAsync(submissionStreamId))
            {
                return NotificationPreferenceUpdateResult.UnsupportedSubmissionStream;
            }

            await _notificationPreferenceRepository.UpsertAsync(
                serviceAreaUserId.Value,
                submissionStreamId,
                update.SuccessfulUploadsEnabled.Value,
                update.UploadErrorsEnabled.Value);

            _unitOfWork.Commit();
            return NotificationPreferenceUpdateResult.Updated;
        }

        public async Task UpdateAllCurrentUserPreferencesAsync(bool enabled)
        {
            EnsureCurrentUserIsEligible();
            await _notificationPreferenceRepository.SetAllAsync(_currentUser.UserInfo.SystemUserId, enabled);
            _unitOfWork.Commit();
        }

        public async Task InitializeDefaultsForInternalUserAsync(decimal systemUserId)
        {
            await _notificationPreferenceRepository.EnsureDefaultsForInternalUserAsync(systemUserId);
            _unitOfWork.Commit();
        }

        private void EnsureCurrentUserIsEligible()
        {
            if (!IsCurrentUserEligible)
            {
                throw new UnauthorizedAccessException("Notification preferences are only available to internal users.");
            }
        }
    }
}
