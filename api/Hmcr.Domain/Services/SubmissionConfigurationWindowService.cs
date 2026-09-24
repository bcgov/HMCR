using Hmcr.Data.Database.Entities;
using Hmcr.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hmcr.Domain.Services
{
    public class SubmissionWindowOccurrence
    {
        public HmrSubmissionConfigWindow Window { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }

    public class SubmissionConfigurationState
    {
        public string State { get; set; }
        public SubmissionWindowOccurrence Occurrence { get; set; }
    }

    public interface ISubmissionConfigurationWindowService
    {
        SubmissionConfigurationState GetState(HmrSubmissionConfiguration configuration, DateOnly localDate);
        SubmissionWindowOccurrence GetNextBlackout(
            HmrSubmissionConfiguration configuration,
            DateOnly localDate);
    }

    public class SubmissionConfigurationWindowService : ISubmissionConfigurationWindowService
    {
        public SubmissionConfigurationState GetState(
            HmrSubmissionConfiguration configuration,
            DateOnly localDate)
        {
            if (!configuration.IsActive)
            {
                return State(SubmissionConfigurationValues.InactiveState);
            }

            if (!IsEffective(configuration, localDate))
            {
                return State(SubmissionConfigurationValues.OutsideWindowState);
            }

            var blackout = FindContaining(configuration, SubmissionConfigurationValues.BlackoutWindow, localDate);
            if (blackout != null)
            {
                return State(SubmissionConfigurationValues.BlackoutState, blackout);
            }

            var reminder = FindContaining(configuration, SubmissionConfigurationValues.ReminderWindow, localDate);
            return reminder == null
                ? State(SubmissionConfigurationValues.OutsideWindowState)
                : State(SubmissionConfigurationValues.ReminderState, reminder);
        }

        public SubmissionWindowOccurrence GetNextBlackout(
            HmrSubmissionConfiguration configuration,
            DateOnly localDate)
        {
            var window = configuration.Windows.FirstOrDefault(x =>
                string.Equals(
                    x.WindowType,
                    SubmissionConfigurationValues.BlackoutWindow,
                    StringComparison.OrdinalIgnoreCase));

            if (window == null)
            {
                return null;
            }

            var effectiveFrom = DateOnly.FromDateTime(configuration.EffectiveFromDate);
            var searchDate = localDate > effectiveFrom ? localDate : effectiveFrom;
            var occurrences = new List<SubmissionWindowOccurrence>();

            for (var year = searchDate.Year - 1; year <= searchDate.Year + 6; year++)
            {
                var occurrence = CreateOccurrence(window, year);
                occurrence = ClipToEffectiveDates(configuration, occurrence);
                if (occurrence != null && occurrence.EndDate >= searchDate)
                {
                    occurrences.Add(occurrence);
                }
            }

            return occurrences
                .OrderBy(x => x.StartDate < searchDate ? searchDate : x.StartDate)
                .ThenBy(x => x.StartDate)
                .FirstOrDefault();
        }

        private static SubmissionWindowOccurrence FindContaining(
            HmrSubmissionConfiguration configuration,
            string windowType,
            DateOnly localDate)
        {
            foreach (var window in configuration.Windows.Where(x =>
                         string.Equals(x.WindowType, windowType, StringComparison.OrdinalIgnoreCase)))
            {
                foreach (var year in new[] { localDate.Year - 1, localDate.Year })
                {
                    var occurrence = ClipToEffectiveDates(configuration, CreateOccurrence(window, year));
                    if (occurrence != null &&
                        localDate >= occurrence.StartDate &&
                        localDate <= occurrence.EndDate)
                    {
                        return occurrence;
                    }
                }
            }

            return null;
        }

        private static SubmissionWindowOccurrence CreateOccurrence(
            HmrSubmissionConfigWindow window,
            int startYear)
        {
            var start = CreateDate(startYear, window.StartMonth, window.StartDay);
            var crossesYear = (window.EndMonth, window.EndDay).CompareTo((window.StartMonth, window.StartDay)) < 0;
            var end = CreateDate(startYear + (crossesYear ? 1 : 0), window.EndMonth, window.EndDay);

            return new SubmissionWindowOccurrence
            {
                Window = window,
                StartDate = start,
                EndDate = end
            };
        }

        private static SubmissionWindowOccurrence ClipToEffectiveDates(
            HmrSubmissionConfiguration configuration,
            SubmissionWindowOccurrence occurrence)
        {
            var effectiveFrom = DateOnly.FromDateTime(configuration.EffectiveFromDate);
            var effectiveTo = configuration.EffectiveToDate.HasValue
                ? DateOnly.FromDateTime(configuration.EffectiveToDate.Value)
                : DateOnly.MaxValue;

            if (occurrence.EndDate < effectiveFrom || occurrence.StartDate > effectiveTo)
            {
                return null;
            }

            return new SubmissionWindowOccurrence
            {
                Window = occurrence.Window,
                StartDate = occurrence.StartDate < effectiveFrom ? effectiveFrom : occurrence.StartDate,
                EndDate = occurrence.EndDate > effectiveTo ? effectiveTo : occurrence.EndDate
            };
        }

        private static bool IsEffective(HmrSubmissionConfiguration configuration, DateOnly date)
        {
            var effectiveFrom = DateOnly.FromDateTime(configuration.EffectiveFromDate);
            var effectiveTo = configuration.EffectiveToDate.HasValue
                ? DateOnly.FromDateTime(configuration.EffectiveToDate.Value)
                : DateOnly.MaxValue;
            return date >= effectiveFrom && date <= effectiveTo;
        }

        private static DateOnly CreateDate(int year, int month, int day)
        {
            var validDay = Math.Min(day, DateTime.DaysInMonth(year, month));
            return new DateOnly(year, month, validDay);
        }

        private static SubmissionConfigurationState State(
            string state,
            SubmissionWindowOccurrence occurrence = null)
        {
            return new SubmissionConfigurationState
            {
                State = state,
                Occurrence = occurrence
            };
        }
    }
}
