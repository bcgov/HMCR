using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Hmcr.Model
{
    public static class MethodLogger
    {
        public static void LogEntry(ILogger logger, bool enabled, string logHeader, string logFooter = "", [CallerMemberName]string name = null)
        {
            if (logger == null || !enabled)
                return;

            logger.LogInformation($"{logHeader} {name} {logFooter}");
        }
    }
}
