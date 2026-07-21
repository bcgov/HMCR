using System;

namespace Hmcr.Model.Logging
{
    public static class HmcrSupportId
    {
        public static string Create()
        {
            return $"HMCR-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant()}";
        }
    }
}
