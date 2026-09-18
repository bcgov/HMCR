using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionConfiguration;
using Hmcr.Model.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface ISubmissionRestrictionRuleEvaluator
    {
        string RuleType { get; }
        bool IsMatch(HmrSubmissionConfigRule rule, SubmissionRestrictionRowDto row);
    }

    public class ActivityAccomplishmentRestrictionRuleEvaluator : ISubmissionRestrictionRuleEvaluator
    {
        public string RuleType => SubmissionConfigurationValues.ActivityAccomplishmentRule;

        public bool IsMatch(HmrSubmissionConfigRule rule, SubmissionRestrictionRowDto row)
        {
            if (!row.Accomplishment.HasValue || string.IsNullOrWhiteSpace(row.ActivityNumber))
            {
                return false;
            }

            var activityMatches = rule.Activities.Any(x =>
                string.Equals(
                    x.ActivityCode?.ActivityNumber?.Trim(),
                    row.ActivityNumber.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (!activityMatches)
            {
                return false;
            }

            if (string.Equals(
                    rule.ComparisonOperator,
                    SubmissionConfigurationValues.GreaterThanOperator,
                    StringComparison.OrdinalIgnoreCase))
            {
                return row.Accomplishment.Value > rule.ThresholdValue;
            }

            if (string.Equals(
                    rule.ComparisonOperator,
                    SubmissionConfigurationValues.GreaterThanOrEqualOperator,
                    StringComparison.OrdinalIgnoreCase))
            {
                return row.Accomplishment.Value >= rule.ThresholdValue;
            }

            return false;
        }
    }

    public interface ISubmissionRestrictionEvaluator
    {
        Task<IReadOnlyList<SubmissionRestrictionViolationDto>> EvaluateAsync(
            decimal submissionStreamId,
            decimal? serviceAreaNumber,
            string userType,
            DateTimeOffset submittedAtUtc,
            IEnumerable<SubmissionRestrictionRowDto> rows);
    }

    public class SubmissionRestrictionEvaluator : ISubmissionRestrictionEvaluator
    {
        private readonly ISubmissionConfigurationRepository _repository;
        private readonly IPacificTimeService _pacificTimeService;
        private readonly ISubmissionConfigurationWindowService _windowService;
        private readonly ISubmissionConfigurationMessageService _messageService;
        private readonly IReadOnlyDictionary<string, ISubmissionRestrictionRuleEvaluator> _ruleEvaluators;

        public SubmissionRestrictionEvaluator(
            ISubmissionConfigurationRepository repository,
            IPacificTimeService pacificTimeService,
            ISubmissionConfigurationWindowService windowService,
            ISubmissionConfigurationMessageService messageService,
            IEnumerable<ISubmissionRestrictionRuleEvaluator> ruleEvaluators)
        {
            _repository = repository;
            _pacificTimeService = pacificTimeService;
            _windowService = windowService;
            _messageService = messageService;
            _ruleEvaluators = ruleEvaluators
                .GroupBy(x => x.RuleType, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
        }

        public async Task<IReadOnlyList<SubmissionRestrictionViolationDto>> EvaluateAsync(
            decimal submissionStreamId,
            decimal? serviceAreaNumber,
            string userType,
            DateTimeOffset submittedAtUtc,
            IEnumerable<SubmissionRestrictionRowDto> rows)
        {
            if (string.Equals(userType, UserTypeDto.INTERNAL, StringComparison.OrdinalIgnoreCase))
            {
                return Array.Empty<SubmissionRestrictionViolationDto>();
            }

            var localDate = _pacificTimeService.GetPacificDate(submittedAtUtc);
            var configurations = await _repository.GetApplicableAsync(
                submissionStreamId,
                serviceAreaNumber,
                activeOnly: true);
            var inputRows = rows?.ToList() ?? new List<SubmissionRestrictionRowDto>();
            var violations = new List<SubmissionRestrictionViolationDto>();
            var seen = new HashSet<(decimal ConfigurationId, int RowNum, string RecordNumber)>();

            foreach (var configuration in configurations)
            {
                if (!AppliesToUserType(configuration, userType))
                {
                    continue;
                }

                var state = _windowService.GetState(configuration, localDate);
                if (!string.Equals(
                        state.State,
                        SubmissionConfigurationValues.BlackoutState,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (var row in inputRows)
                {
                    if (!configuration.Rules.Any(rule => IsMatch(rule, row)))
                    {
                        continue;
                    }

                    var key = (configuration.SubmissionConfigurationId, row.RowNum, row.RecordNumber ?? string.Empty);
                    if (!seen.Add(key))
                    {
                        continue;
                    }

                    violations.Add(new SubmissionRestrictionViolationDto
                    {
                        SubmissionConfigurationId = configuration.SubmissionConfigurationId,
                        ConfigurationKey = configuration.ConfigurationKey,
                        RowNum = row.RowNum,
                        RecordNumber = row.RecordNumber,
                        ActivityNumber = row.ActivityNumber,
                        Message = _messageService.BuildViolationMessage(row, state.Occurrence)
                    });
                }
            }

            return violations
                .OrderBy(x => x.RowNum)
                .ThenBy(x => x.SubmissionConfigurationId)
                .ToList();
        }

        private bool IsMatch(HmrSubmissionConfigRule rule, SubmissionRestrictionRowDto row)
        {
            return _ruleEvaluators.TryGetValue(rule.RuleType, out var evaluator) &&
                   evaluator.IsMatch(rule, row);
        }

        private static bool AppliesToUserType(
            HmrSubmissionConfiguration configuration,
            string userType)
        {
            return configuration.Audiences.Any(audience =>
                string.Equals(
                    audience.AudienceType,
                    SubmissionConfigurationValues.UserTypeAudience,
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(audience.AudienceValue, userType, StringComparison.OrdinalIgnoreCase));
        }
    }
}
