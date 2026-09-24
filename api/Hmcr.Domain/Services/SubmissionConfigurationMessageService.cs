using Hmcr.Data.Database.Entities;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionConfiguration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Hmcr.Domain.Services
{
    public interface ISubmissionConfigurationMessageService
    {
        string BuildNotice(
            HmrSubmissionConfiguration configuration,
            string phase,
            SubmissionWindowOccurrence blackout);
        string BuildViolationMessage(
            SubmissionRestrictionRowDto row,
            SubmissionWindowOccurrence blackout);
    }

    public class SubmissionConfigurationMessageService : ISubmissionConfigurationMessageService
    {
        public string BuildNotice(
            HmrSubmissionConfiguration configuration,
            string phase,
            SubmissionWindowOccurrence blackout)
        {
            if (blackout == null)
            {
                return null;
            }

            var range = $"{FormatDate(blackout.StartDate)} through {FormatDate(blackout.EndDate)}";

            if (string.Equals(phase, SubmissionConfigurationValues.ReminderState, StringComparison.OrdinalIgnoreCase))
            {
                var ruleSummary = BuildRuleSummary(configuration.Rules, "and");
                var deadline = blackout.StartDate.AddDays(-1);
                return $"Reminder: Work reports for {ruleSummary} must be submitted by {FormatDate(deadline)}. " +
                       $"From {range}, contractors cannot submit these records. " +
                       "Contact Ministry staff if a late submission is required.";
            }

            if (string.Equals(phase, SubmissionConfigurationValues.BlackoutState, StringComparison.OrdinalIgnoreCase))
            {
                var ruleSummary = BuildRuleSummary(configuration.Rules, "or");
                return $"Submission blackout in effect: From {range}, maintenance contractors cannot submit " +
                       $"work reports for {ruleSummary}. Contact Ministry staff if a late submission is required.";
            }

            return null;
        }

        public string BuildViolationMessage(
            SubmissionRestrictionRowDto row,
            SubmissionWindowOccurrence blackout)
        {
            return $"Row {row.RowNum}, record {row.RecordNumber}: This record cannot be submitted by a contractor " +
                   $"between {FormatDate(blackout.StartDate)} and {FormatDate(blackout.EndDate)} because the " +
                   "reported activity meets the capitalization threshold. Contact Ministry staff to submit the " +
                   "record on your behalf.";
        }

        private static string BuildRuleSummary(
            IEnumerable<HmrSubmissionConfigRule> rules,
            string conjunction)
        {
            var phrases = rules
                .OrderBy(x => x.DisplayOrder)
                .Select(rule =>
                    $"{LowercaseFirst(rule.DisplayLabel)} {FormatComparison(rule)}")
                .ToList();

            if (phrases.Count == 0)
            {
                return "configured activities";
            }

            if (phrases.Count == 1)
            {
                return phrases[0];
            }

            if (phrases.Count == 2)
            {
                return $"{phrases[0]} {conjunction} {phrases[1]}";
            }

            return $"{string.Join(", ", phrases.Take(phrases.Count - 1))}, {conjunction} {phrases[^1]}";
        }

        private static string FormatComparison(HmrSubmissionConfigRule rule)
        {
            var value = rule.ThresholdValue.ToString("#,0.####", CultureInfo.InvariantCulture);
            var unit = FormatUnit(rule.UnitOfMeasure);
            return string.Equals(
                    rule.ComparisonOperator,
                    SubmissionConfigurationValues.GreaterThanOperator,
                    StringComparison.OrdinalIgnoreCase)
                ? $"exceeding {value} {unit}"
                : $"of {value} {unit} or more";
        }

        private static string FormatUnit(string unit)
        {
            if (string.Equals(unit, "tonne", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(unit, "tonnes", StringComparison.OrdinalIgnoreCase))
            {
                return "tonnes";
            }

            if (string.Equals(unit, "m2", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(unit, "m²", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(unit, "square metres", StringComparison.OrdinalIgnoreCase))
            {
                return "m²";
            }

            return unit;
        }

        private static string LowercaseFirst(string value)
        {
            if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
            {
                return value;
            }

            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        private static string FormatDate(DateOnly date)
        {
            return date.ToString("MMMM d", CultureInfo.InvariantCulture);
        }
    }
}
