using Hangfire.Common;
using System;
using System.Linq;
using Newtonsoft.Json;
using Hangfire.Client;
using Hangfire.Server;

namespace Hmcr.Domain.Hangfire
{
    /// <summary>
    /// This prevents jobs for the same service area from running concurrently
    /// 
    /// However, it's not complete because when there are more than one Hangfire server,
    /// more than one job instance may get the same monitor query result of "there's no job running for the service area".
    /// In that case, more than one jobs can run conccurrently.
    /// Locking using sp_getapplock is one solution but not an ideal solution. (ex. DisableConcurrentExecution in Hangfire library)
    /// 
    /// The solution implemented in this app is to follow optimistic concurrency control approach and 
    /// it's implemented by handling the result of SubmissionObjectRepository.UpdateSubmissionStatusAsync() 
    /// in the SubmissionObjectJobService.RunReportingJob().
    /// </summary>
    public sealed class SkipSameJobAttribute : JobFilterAttribute, IClientFilter
    {
        public void OnCreated(CreatedContext filterContext)
        {
        }

        public void OnCreating(CreatingContext context)
        {
            var job = context.Job;
            var jobFingerprint = GetJobFingerprint(job);

            var monitor = context.Storage.GetMonitoringApi();
            var fingerprints = monitor.ProcessingJobs(0, 10000)
                .Select(x => GetJobFingerprint(x.Value.Job))
                .ToList();

            fingerprints.AddRange(
                monitor.EnqueuedJobs("default", 0, 10000)
                .Select(x => GetJobFingerprint(x.Value.Job))
            );

            foreach (var fingerprint in fingerprints)
            {
                if (jobFingerprint != fingerprint)
                    continue;

                context.Canceled = true;

                Console.WriteLine($"[Hangfire] Cancelling a job {jobFingerprint}");

                return;
            }
        }

        private string GetJobFingerprint(Job job)
        {
            return $"{job.Type.FullName}-{job.Method.Name}-{JsonConvert.SerializeObject(job.Args)}";
        }
    }
}
