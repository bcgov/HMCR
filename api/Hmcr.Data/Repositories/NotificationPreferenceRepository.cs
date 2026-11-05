using Hmcr.Data.Database.Entities;
using Hmcr.Model;
using Hmcr.Model.Dtos.NotificationPreference;
using Hmcr.Model.Dtos.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface INotificationPreferenceRepository
    {
        Task<NotificationPreferencesDto> GetForUserAsync(decimal systemUserId);
        Task<decimal?> GetActiveServiceAreaUserIdAsync(decimal systemUserId, decimal serviceAreaNumber);
        Task<bool> IsSupportedSubmissionStreamAsync(decimal submissionStreamId);
        Task UpsertAsync(decimal serviceAreaUserId, decimal submissionStreamId, bool successfulUploadsEnabled, bool uploadErrorsEnabled);
        Task SetAllAsync(decimal systemUserId, bool enabled);
        Task EnsureDefaultsForInternalUserAsync(decimal systemUserId);
    }

    public class NotificationPreferenceRepository : INotificationPreferenceRepository
    {
        private static readonly string[] SupportedStagingTableNames =
        {
            TableNames.WorkReport,
            TableNames.RockfallReport,
            TableNames.WildlifeReport
        };

        private readonly AppDbContext _dbContext;

        public NotificationPreferenceRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<NotificationPreferencesDto> GetForUserAsync(decimal systemUserId)
        {
            var serviceAreaUsers = await GetActiveServiceAreaUsersQuery(systemUserId)
                .AsNoTracking()
                .Include(x => x.ServiceAreaNumberNavigation)
                .OrderBy(x => x.ServiceAreaNumber)
                .ToListAsync();

            var streams = await GetSupportedSubmissionStreamsQuery()
                .AsNoTracking()
                .ToListAsync();

            streams = streams
                .OrderBy(x => Array.IndexOf(SupportedStagingTableNames, x.StagingTableName))
                .ToList();

            var serviceAreaUserIds = serviceAreaUsers.Select(x => x.ServiceAreaUserId).ToArray();
            var streamIds = streams.Select(x => x.SubmissionStreamId).ToArray();

            var savedPreferences = await _dbContext.HmrNotificationPreferences
                .AsNoTracking()
                .Where(x => serviceAreaUserIds.Contains(x.ServiceAreaUserId) && streamIds.Contains(x.SubmissionStreamId))
                .ToListAsync();

            var savedPreferencesByTarget = savedPreferences.ToDictionary(
                x => (x.ServiceAreaUserId, x.SubmissionStreamId));

            var response = new NotificationPreferencesDto();

            foreach (var serviceAreaUser in serviceAreaUsers)
            {
                var serviceArea = new NotificationPreferenceServiceAreaDto
                {
                    ServiceAreaNumber = serviceAreaUser.ServiceAreaNumber,
                    ServiceAreaName = serviceAreaUser.ServiceAreaNumberNavigation.ServiceAreaName
                };

                foreach (var stream in streams)
                {
                    savedPreferencesByTarget.TryGetValue(
                        (serviceAreaUser.ServiceAreaUserId, stream.SubmissionStreamId),
                        out var savedPreference);

                    serviceArea.ReportTypes.Add(new NotificationPreferenceReportTypeDto
                    {
                        SubmissionStreamId = stream.SubmissionStreamId,
                        ReportTypeName = stream.StreamName,
                        StagingTableName = stream.StagingTableName,
                        SuccessfulUploadsEnabled = savedPreference?.SuccessEmailEnabled ?? true,
                        UploadErrorsEnabled = savedPreference?.ErrorEmailEnabled ?? true
                    });
                }

                response.ServiceAreas.Add(serviceArea);
            }

            return response;
        }

        public async Task<decimal?> GetActiveServiceAreaUserIdAsync(decimal systemUserId, decimal serviceAreaNumber)
        {
            return await GetActiveServiceAreaUsersQuery(systemUserId)
                .Where(x => x.ServiceAreaNumber == serviceAreaNumber)
                .Select(x => (decimal?)x.ServiceAreaUserId)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> IsSupportedSubmissionStreamAsync(decimal submissionStreamId)
        {
            return await GetSupportedSubmissionStreamsQuery()
                .AnyAsync(x => x.SubmissionStreamId == submissionStreamId);
        }

        public async Task UpsertAsync(
            decimal serviceAreaUserId,
            decimal submissionStreamId,
            bool successfulUploadsEnabled,
            bool uploadErrorsEnabled)
        {
            var preference = await _dbContext.HmrNotificationPreferences
                .SingleOrDefaultAsync(x =>
                    x.ServiceAreaUserId == serviceAreaUserId &&
                    x.SubmissionStreamId == submissionStreamId);

            if (preference == null)
            {
                preference = new HmrNotificationPreference
                {
                    ServiceAreaUserId = serviceAreaUserId,
                    SubmissionStreamId = submissionStreamId
                };

                await _dbContext.HmrNotificationPreferences.AddAsync(preference);
            }

            preference.SuccessEmailEnabled = successfulUploadsEnabled;
            preference.ErrorEmailEnabled = uploadErrorsEnabled;
        }

        public async Task SetAllAsync(decimal systemUserId, bool enabled)
        {
            var activeServiceAreaUserIds = await GetActiveServiceAreaUsersQuery(systemUserId)
                .Select(x => x.ServiceAreaUserId)
                .ToListAsync();

            var supportedStreamIds = await GetSupportedSubmissionStreamsQuery()
                .Select(x => x.SubmissionStreamId)
                .ToListAsync();

            var existingPreferences = await _dbContext.HmrNotificationPreferences
                .Where(x =>
                    activeServiceAreaUserIds.Contains(x.ServiceAreaUserId) &&
                    supportedStreamIds.Contains(x.SubmissionStreamId))
                .ToListAsync();

            foreach (var preference in existingPreferences)
            {
                preference.SuccessEmailEnabled = enabled;
                preference.ErrorEmailEnabled = enabled;
            }

            var existingTargets = existingPreferences
                .Select(x => (x.ServiceAreaUserId, x.SubmissionStreamId))
                .ToHashSet();

            foreach (var serviceAreaUserId in activeServiceAreaUserIds)
            {
                foreach (var submissionStreamId in supportedStreamIds)
                {
                    if (existingTargets.Contains((serviceAreaUserId, submissionStreamId)))
                    {
                        continue;
                    }

                    await _dbContext.HmrNotificationPreferences.AddAsync(new HmrNotificationPreference
                    {
                        ServiceAreaUserId = serviceAreaUserId,
                        SubmissionStreamId = submissionStreamId,
                        SuccessEmailEnabled = enabled,
                        ErrorEmailEnabled = enabled
                    });
                }
            }
        }

        public async Task EnsureDefaultsForInternalUserAsync(decimal systemUserId)
        {
            var isInternal = await _dbContext.HmrSystemUsers
                .AnyAsync(x => x.SystemUserId == systemUserId && x.UserType == UserTypeDto.INTERNAL);

            if (!isInternal)
            {
                return;
            }

            var activeServiceAreaUserIds = await GetActiveServiceAreaUsersQuery(systemUserId)
                .Select(x => x.ServiceAreaUserId)
                .ToListAsync();

            var supportedStreamIds = await GetSupportedSubmissionStreamsQuery()
                .Select(x => x.SubmissionStreamId)
                .ToListAsync();

            var existingTargets = await _dbContext.HmrNotificationPreferences
                .Where(x =>
                    activeServiceAreaUserIds.Contains(x.ServiceAreaUserId) &&
                    supportedStreamIds.Contains(x.SubmissionStreamId))
                .Select(x => new { x.ServiceAreaUserId, x.SubmissionStreamId })
                .ToListAsync();

            var existingTargetSet = existingTargets
                .Select(x => (x.ServiceAreaUserId, x.SubmissionStreamId))
                .ToHashSet();

            foreach (var serviceAreaUserId in activeServiceAreaUserIds)
            {
                foreach (var submissionStreamId in supportedStreamIds)
                {
                    if (existingTargetSet.Contains((serviceAreaUserId, submissionStreamId)))
                    {
                        continue;
                    }

                    await _dbContext.HmrNotificationPreferences.AddAsync(new HmrNotificationPreference
                    {
                        ServiceAreaUserId = serviceAreaUserId,
                        SubmissionStreamId = submissionStreamId,
                        SuccessEmailEnabled = true,
                        ErrorEmailEnabled = true
                    });
                }
            }
        }

        private IQueryable<HmrServiceAreaUser> GetActiveServiceAreaUsersQuery(decimal systemUserId)
        {
            return _dbContext.HmrServiceAreaUsers.Where(x =>
                x.SystemUserId == systemUserId &&
                (x.EndDate == null || x.EndDate > DateTime.Today));
        }

        private IQueryable<HmrSubmissionStream> GetSupportedSubmissionStreamsQuery()
        {
            return _dbContext.HmrSubmissionStreams.Where(x =>
                SupportedStagingTableNames.Contains(x.StagingTableName));
        }
    }
}
