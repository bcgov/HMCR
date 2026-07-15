namespace Hmcr.Model.Logging
{
    public static class HmcrLogConstants
    {
        public const string CorrelationHeader = "X-HMCR-Correlation-ID";
        public const string SupportIdHeader = "X-HMCR-Support-ID";

        public static class Sources
        {
            public const string Api = "api";
            public const string Client = "client";
            public const string Hangfire = "hangfire";
        }

        public static class ActorTypes
        {
            public const string Anonymous = "anonymous";
            public const string User = "user";
            public const string System = "system";
        }

        public static class ErrorCodes
        {
            public const string ApiUnexpected = "HMCR-API-UNEXPECTED";
            public const string ClientUnexpected = "HMCR-CLIENT-UNEXPECTED";
            public const string HangfireUnexpected = "HMCR-HANGFIRE-UNEXPECTED";
        }
    }
}
