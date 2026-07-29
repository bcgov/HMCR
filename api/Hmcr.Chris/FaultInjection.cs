using System;
using Microsoft.Extensions.Configuration;

namespace Hmcr.Chris
{
    /// <summary>
    /// Lets a tester reproduce each class of 'Unexpected Error' / system-error on demand by
    /// setting FaultInjection:Enabled and FaultInjection:Scenario in the OpenShift configmap
    /// (FAULT_INJECTION_ENABLED / FAULT_INJECTION_SCENARIO template params) and restarting the
    /// hangfire pod - no code change or VPN/CHRIS access required. See Scenario for the map of
    /// config value -> simulated failure -> where it's injected.
    ///
    /// Hard-blocked in Production regardless of config, so a stray configmap edit cannot affect
    /// real submissions there.
    /// </summary>
    public static class FaultInjection
    {
        /// <summary>
        /// Config value -> simulated failure -> injection point:
        ///   None                 - disabled, no injection (default).
        ///   ChrisHtmlResponse    - CHRIS/GeoServer returns an HTML error page with HTTP 200.
        ///                          Injected in OasApi.GetRfiSegmentDetailAsync; exercises
        ///                          ApiResponseGuard.
        ///   ChrisUnauthorized    - CHRIS rejects the service account (401). Injected in
        ///                          OasApi.GetRfiSegmentDetailAsync as a thrown exception with
        ///                          the same message Api.SendWithRetry produces for a real 401.
        ///   ChrisTimeout         - CHRIS is unreachable after retries. Injected the same way,
        ///                          with the same message Api.SendWithRetry produces on timeout.
        ///   ChrisNullLength      - CHRIS returns a valid feature with NE_LENGTH: null (the real
        ///                          defect seen on segment 13-D-D-00001W). Injected as synthetic
        ///                          JSON so the real nullable-Property/RfiSegment deserialization
        ///                          and zero-length LRS handling run against it.
        ///   UnhandledException  - a generic unhandled exception (e.g. NullReferenceException)
        ///                          escapes submission processing. Injected in
        ///                          ReportJobServiceBase.SetSubmissionAsync; exercises the UE
        ///                          catch-all path and GetUnexpectedErrorDetail.
        ///   AggregateAsyncFailure - parallel row validation fails with an AggregateException.
        ///                          Injected the same way; exercises the AggregateException
        ///                          flattening in GetUnexpectedErrorDetail.
        /// </summary>
        public static class Scenario
        {
            public const string None = "None";
            public const string ChrisHtmlResponse = "ChrisHtmlResponse";
            public const string ChrisUnauthorized = "ChrisUnauthorized";
            public const string ChrisTimeout = "ChrisTimeout";
            public const string ChrisNullLength = "ChrisNullLength";
            public const string UnhandledException = "UnhandledException";
            public const string AggregateAsyncFailure = "AggregateAsyncFailure";
        }

        private const string ProductionEnvironmentName = "PRODUCTION";
        private const string SimulatedTag = "[Simulated via FaultInjection:Scenario={0}]";

        /// <summary>
        /// Returns the active scenario, or Scenario.None if fault injection is disabled, unset,
        /// or the environment is Production (checked directly from the environment variable, not
        /// configuration, so it cannot be bypassed by a configmap that also overrides
        /// ASPNETCORE_ENVIRONMENT in appsettings).
        /// </summary>
        public static string ActiveScenario(IConfiguration config)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.Equals(envName, ProductionEnvironmentName, StringComparison.OrdinalIgnoreCase))
                return Scenario.None;

            if (config == null || !config.GetValue<bool>("FaultInjection:Enabled"))
                return Scenario.None;

            return config["FaultInjection:Scenario"] ?? Scenario.None;
        }

        public static bool Is(IConfiguration config, string scenario) =>
            string.Equals(ActiveScenario(config), scenario, StringComparison.OrdinalIgnoreCase);

        public static string Tag(string scenario) => string.Format(SimulatedTag, scenario);
    }
}
