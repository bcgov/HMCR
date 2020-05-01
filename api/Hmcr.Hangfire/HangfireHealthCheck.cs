using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Hmcr.Hangfire
{
    public class HangfireHealthCheck : IHealthCheck
    {
        private string _connectionString;
        private IMonitoringApi _monitoringApi;

        public HangfireHealthCheck(string connectionString)
        {
            _connectionString = connectionString;
            _monitoringApi = JobStorage.Current.GetMonitoringApi();
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                await connection.OpenAsync(cancellationToken);

                var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM HMR_SERVICE_AREA";

                var count = await command.ExecuteScalarAsync(cancellationToken);
                var jobCount = Convert.ToInt32(count) + 1; //one job per service area + one job for resending email

                var stats = _monitoringApi.GetStatistics();
                var statsJson = JsonSerializer.Serialize(stats);

                if (stats.Servers > 0 && stats.Recurring >= jobCount)
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
