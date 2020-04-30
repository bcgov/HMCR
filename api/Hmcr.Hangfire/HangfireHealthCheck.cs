using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Hmcr.Hangfire
{
    public class HangfireHealthCheck : IHealthCheck
    {
        private int _numJobs;
        private IMonitoringApi _monitoringApi;

        public HangfireHealthCheck(int numJobs)
        {
            _numJobs = numJobs;
            _monitoringApi = JobStorage.Current.GetMonitoringApi();
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.CompletedTask;

                var stats = _monitoringApi.GetStatistics();
                var statsJson = JsonSerializer.Serialize(stats);

                if (stats.Servers > 0 && stats.Recurring >= _numJobs)
                {
                    return HealthCheckResult.Healthy(statsJson);
                }
                else
                {
                    return HealthCheckResult.Unhealthy(statsJson);
                }

            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
