using Hmcr.Model;
using System;

namespace Hmcr.Domain.Services
{
    public interface IPacificTimeService
    {
        DateTimeOffset GetCurrentPacificTime();
        DateTimeOffset ConvertToPacific(DateTimeOffset timestamp);
        DateOnly GetPacificDate(DateTimeOffset timestamp);
    }

    public class PacificTimeService : IPacificTimeService
    {
        private readonly TimeProvider _timeProvider;
        private readonly TimeZoneInfo _pacificTimeZone;

        public PacificTimeService(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
            _pacificTimeZone = ResolvePacificTimeZone();
        }

        public DateTimeOffset GetCurrentPacificTime()
        {
            return ConvertToPacific(_timeProvider.GetUtcNow());
        }

        public DateTimeOffset ConvertToPacific(DateTimeOffset timestamp)
        {
            return TimeZoneInfo.ConvertTime(timestamp, _pacificTimeZone);
        }

        public DateOnly GetPacificDate(DateTimeOffset timestamp)
        {
            return DateOnly.FromDateTime(ConvertToPacific(timestamp).DateTime);
        }

        private static TimeZoneInfo ResolvePacificTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(Constants.VancouverTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById(Constants.PacificTimeZone);
            }
        }
    }
}
