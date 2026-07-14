using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Chris
{
    public interface IApi
    {
        Task<HttpResponseMessage> Get(HttpClient client, string path);
        Task<HttpResponseMessage> Post(HttpClient client, string path, string body);
        Task<HttpResponseMessage> GetWithRetry(HttpClient client, string path);
        Task<HttpResponseMessage> PostWithRetry(HttpClient client, string path, string body);

    }
    public class Api : IApi
    {
        const int maxAttempt = 5;

        public async Task<HttpResponseMessage> Get(HttpClient client, string path)
        {
            var response = await client.GetAsync(path);

            return response;
        }

        public async Task<HttpResponseMessage> Post(HttpClient client, string path, string body)
        {
            var response
                = await client.PostAsync(path, new StringContent(body, Encoding.UTF8));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Status Code: {response.StatusCode}");
            }

            return response;
        }

        public Task<HttpResponseMessage> GetWithRetry(HttpClient client, string path)
        {
            return SendWithRetry(() => client.GetAsync(path));
        }

        public Task<HttpResponseMessage> PostWithRetry(HttpClient client, string path, string body)
        {
            return SendWithRetry(() => client.PostAsync(path, new StringContent(body, Encoding.UTF8)));
        }

        /// <summary>
        /// Sends a request up to maxAttempt times with exponential backoff.
        /// Retries both non-success status codes AND transient transport exceptions
        /// (connection failures, timeouts). The previous implementation only retried
        /// bad status codes - a thrown HttpRequestException or timeout on the first
        /// attempt bypassed the retry loop entirely and failed the whole submission.
        /// </summary>
        private static async Task<HttpResponseMessage> SendWithRetry(Func<Task<HttpResponseMessage>> send)
        {
            Exception lastException = null;
            HttpResponseMessage response = null;

            for (var attempt = 1; attempt <= maxAttempt; attempt++)
            {
                if (attempt > 1)
                {
                    await Task.Delay(200 * (int)Math.Pow(2, attempt - 2)); // 200ms, 400ms, 800ms, 1600ms
                }

                try
                {
                    lastException = null;
                    response = await send();

                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }

                    //credential failures are not transient - retrying only delays the inevitable
                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var statusCode = response.StatusCode;
                        response.Dispose();
                        throw new Exception($"The mapping service rejected the system's credentials (HTTP {(int)statusCode} {statusCode}). " +
                            "The CHRIS service account may be locked, expired, or misconfigured. " +
                            "Submitters cannot fix this - please contact the HMCR administrator.");
                    }

                    if (attempt < maxAttempt)
                    {
                        response.Dispose();
                        response = null;
                    }
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                }
                catch (TaskCanceledException ex) // HttpClient timeout
                {
                    lastException = ex;
                }
            }

            if (lastException != null)
            {
                throw new Exception($"The mapping service could not be reached after {maxAttempt} attempts: {lastException.Message}", lastException);
            }

            var message = "";
            var finalStatusCode = response.StatusCode;

            try
            {
                if (response.Content != null)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    //error bodies are often HTML pages; reduce them to a readable snippet
                    message = ApiResponseGuard.ExtractText(Encoding.UTF8.GetString(bytes));
                }
            }
            finally
            {
                response.Dispose();
            }

            throw new Exception($"The mapping service returned an error (HTTP {(int)finalStatusCode} {finalStatusCode}) after {maxAttempt} attempts. " +
                "Please submit the file again later; if the problem persists, contact the HMCR administrator. " +
                $"Service response: {message}");
        }
    }
}
