using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionConfiguration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public enum SubmissionConfigurationActivationStatus
    {
        Updated,
        NotFound,
        Conflict
    }

    public class SubmissionConfigurationActivationResult
    {
        public SubmissionConfigurationActivationStatus Status { get; set; }
        public SubmissionConfigurationDto Configuration { get; set; }
    }

    public interface ISubmissionConfigurationService
    {
        Task<IReadOnlyList<SubmissionConfigurationDto>> GetAllAsync();
        Task<SubmissionConfigurationActivationResult> UpdateActivationAsync(
            decimal submissionConfigurationId,
            bool isActive,
            long concurrencyControlNumber);
        Task<IReadOnlyList<SubmissionConfigurationNoticeDto>> GetNoticesAsync(
            decimal submissionStreamId,
            decimal? serviceAreaNumber);
    }

    public class SubmissionConfigurationService : ISubmissionConfigurationService
    {
        private readonly ISubmissionConfigurationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPacificTimeService _pacificTimeService;
        private readonly ISubmissionConfigurationWindowService _windowService;
        private readonly ISubmissionConfigurationMessageService _messageService;

        public SubmissionConfigurationService(
            ISubmissionConfigurationRepository repository,
            IUnitOfWork unitOfWork,
            IPacificTimeService pacificTimeService,
            ISubmissionConfigurationWindowService windowService,
            ISubmissionConfigurationMessageService messageService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _pacificTimeService = pacificTimeService;
            _windowService = windowService;
            _messageService = messageService;
        }

        public async Task<IReadOnlyList<SubmissionConfigurationDto>> GetAllAsync()
        {
            var localDate = DateOnly.FromDateTime(_pacificTimeService.GetCurrentPacificTime().DateTime);
            var configurations = await _repository.GetAllAsync();
            return configurations.Select(x => MapConfiguration(x, localDate)).ToList();
        }

        public async Task<SubmissionConfigurationActivationResult> UpdateActivationAsync(
            decimal submissionConfigurationId,
            bool isActive,
            long concurrencyControlNumber)
        {
            var configuration = await _repository.GetByIdForUpdateAsync(submissionConfigurationId);
            if (configuration == null)
            {
                return Result(SubmissionConfigurationActivationStatus.NotFound);
            }

            if (configuration.ConcurrencyControlNumber != concurrencyControlNumber)
            {
                return Result(SubmissionConfigurationActivationStatus.Conflict);
            }

            configuration.IsActive = isActive;

            try
            {
                _unitOfWork.Commit();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result(SubmissionConfigurationActivationStatus.Conflict);
            }

            var localDate = DateOnly.FromDateTime(_pacificTimeService.GetCurrentPacificTime().DateTime);
            return Result(
                SubmissionConfigurationActivationStatus.Updated,
                MapConfiguration(configuration, localDate));
        }

        public async Task<IReadOnlyList<SubmissionConfigurationNoticeDto>> GetNoticesAsync(
            decimal submissionStreamId,
            decimal? serviceAreaNumber)
        {
            var localDate = DateOnly.FromDateTime(_pacificTimeService.GetCurrentPacificTime().DateTime);
            var configurations = await _repository.GetApplicableAsync(
                submissionStreamId,
                serviceAreaNumber,
                activeOnly: true);
            var notices = new List<SubmissionConfigurationNoticeDto>();

            foreach (var configuration in configurations)
            {
                var state = _windowService.GetState(configuration, localDate);
                if (state.Occurrence == null ||
                    (!string.Equals(
                         state.State,
                         SubmissionConfigurationValues.ReminderState,
                         StringComparison.OrdinalIgnoreCase) &&
                     !string.Equals(
                         state.State,
                         SubmissionConfigurationValues.BlackoutState,
                         StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var blackout = string.Equals(
                    state.State,
                    SubmissionConfigurationValues.BlackoutState,
                    StringComparison.OrdinalIgnoreCase)
                    ? state.Occurrence
                    : _windowService.GetNextBlackout(configuration, localDate);
                var message = _messageService.BuildNotice(configuration, state.State, blackout);
                if (string.IsNullOrWhiteSpace(message))
                {
                    continue;
                }

                notices.Add(new SubmissionConfigurationNoticeDto
                {
                    SubmissionConfigurationId = configuration.SubmissionConfigurationId,
                    ConfigurationKey = configuration.ConfigurationKey,
                    Name = configuration.ConfigurationName,
                    Phase = state.State,
                    Severity = GetSeverity(state.State),
                    Message = message,
                    StartDate = ToDateTime(state.Occurrence.StartDate),
                    EndDate = ToDateTime(state.Occurrence.EndDate)
                });
            }

            return notices;
        }

        private SubmissionConfigurationDto MapConfiguration(
            HmrSubmissionConfiguration configuration,
            DateOnly localDate)
        {
            var state = _windowService.GetState(configuration, localDate);
            var nextBlackoutSearchDate = string.Equals(
                    state.State,
                    SubmissionConfigurationValues.BlackoutState,
                    StringComparison.OrdinalIgnoreCase) &&
                state.Occurrence != null
                    ? state.Occurrence.EndDate.AddDays(1)
                    : localDate;
            var nextBlackout = _windowService.GetNextBlackout(configuration, nextBlackoutSearchDate);
            var noticeBlackout = string.Equals(
                state.State,
                SubmissionConfigurationValues.BlackoutState,
                StringComparison.OrdinalIgnoreCase)
                ? state.Occurrence
                : nextBlackout;
            var generatedNotice = state.Occurrence == null
                ? null
                : _messageService.BuildNotice(configuration, state.State, noticeBlackout);

            return new SubmissionConfigurationDto
            {
                SubmissionConfigurationId = configuration.SubmissionConfigurationId,
                ConfigurationKey = configuration.ConfigurationKey,
                Name = configuration.ConfigurationName,
                SubmissionStreamId = configuration.SubmissionStreamId,
                SubmissionStreamName = configuration.SubmissionStream?.StreamName,
                IsActive = configuration.IsActive,
                EffectiveStartDate = configuration.EffectiveFromDate,
                EffectiveEndDate = configuration.EffectiveToDate,
                TimeZone = configuration.TimeZoneId,
                ScopeType = configuration.ScopeType,
                CurrentState = state.State,
                GeneratedNotice = generatedNotice,
                NoticeSeverity = generatedNotice == null ? null : GetSeverity(state.State),
                CurrentWindowStartDate = state.Occurrence == null
                    ? null
                    : ToDateTime(state.Occurrence.StartDate),
                CurrentWindowEndDate = state.Occurrence == null
                    ? null
                    : ToDateTime(state.Occurrence.EndDate),
                NextBlackoutStartDate = nextBlackout == null ? null : ToDateTime(nextBlackout.StartDate),
                NextBlackoutEndDate = nextBlackout == null ? null : ToDateTime(nextBlackout.EndDate),
                ConcurrencyControlNumber = configuration.ConcurrencyControlNumber,
                LastUpdatedBy = configuration.AppLastUpdateUserid,
                LastUpdatedTimestamp = configuration.AppLastUpdateTimestamp,
                Windows = configuration.Windows
                    .OrderBy(x => x.WindowType)
                    .Select(MapWindow)
                    .ToList(),
                Rules = configuration.Rules
                    .OrderBy(x => x.DisplayOrder)
                    .Select(MapRule)
                    .ToList(),
                Audiences = configuration.Audiences
                    .OrderBy(x => x.AudienceType)
                    .ThenBy(x => x.AudienceValue)
                    .Select(x => new SubmissionConfigurationAudienceDto
                    {
                        AudienceType = x.AudienceType,
                        AudienceValue = x.AudienceValue
                    })
                    .ToList(),
                ServiceAreaNumbers = configuration.ServiceAreas
                    .Select(x => x.ServiceAreaNumber)
                    .OrderBy(x => x)
                    .ToList()
            };
        }

        private static SubmissionConfigurationWindowDto MapWindow(HmrSubmissionConfigWindow window)
        {
            return new SubmissionConfigurationWindowDto
            {
                SubmissionConfigWindowId = window.SubmissionConfigWindowId,
                WindowType = window.WindowType,
                StartMonth = window.StartMonth,
                StartDay = window.StartDay,
                EndMonth = window.EndMonth,
                EndDay = window.EndDay
            };
        }

        private static SubmissionConfigurationRuleDto MapRule(HmrSubmissionConfigRule rule)
        {
            return new SubmissionConfigurationRuleDto
            {
                SubmissionConfigRuleId = rule.SubmissionConfigRuleId,
                RuleType = rule.RuleType,
                DisplayLabel = rule.DisplayLabel,
                ComparisonOperator = rule.ComparisonOperator,
                ThresholdValue = rule.ThresholdValue,
                UnitOfMeasure = rule.UnitOfMeasure,
                Activities = rule.Activities
                    .OrderBy(x => x.ActivityCode?.ActivityNumber)
                    .Select(x => new SubmissionConfigurationActivityDto
                    {
                        ActivityCodeId = x.ActivityCodeId,
                        ActivityNumber = x.ActivityCode?.ActivityNumber,
                        ActivityName = x.ActivityCode?.ActivityName
                    })
                    .ToList()
            };
        }

        private static SubmissionConfigurationActivationResult Result(
            SubmissionConfigurationActivationStatus status,
            SubmissionConfigurationDto configuration = null)
        {
            return new SubmissionConfigurationActivationResult
            {
                Status = status,
                Configuration = configuration
            };
        }

        private static string GetSeverity(string state)
        {
            return string.Equals(
                state,
                SubmissionConfigurationValues.BlackoutState,
                StringComparison.OrdinalIgnoreCase)
                ? SubmissionConfigurationValues.DangerSeverity
                : SubmissionConfigurationValues.WarningSeverity;
        }

        private static DateTime ToDateTime(DateOnly date)
        {
            return date.ToDateTime(TimeOnly.MinValue);
        }
    }
}
