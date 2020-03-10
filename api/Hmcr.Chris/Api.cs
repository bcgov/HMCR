using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Chris
{
    public interface IApi
    {
        Task<string> Get(HttpClient client, string path);
        Task<string> Post(HttpClient client, string path, string body);

    }
    public class Api : IApi
    {
        const int maxAttempt = 5;

        public async Task<string> Get(HttpClient client, string path)
        {
            var response = await client.GetAsync(path);

            if (!response.IsSuccessStatusCode)
            {
                for (var attempt = 2; attempt <= maxAttempt; attempt++)
                {
                    await Task.Delay(100 * attempt);

                    response = await client.GetAsync(path);

                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }
                    else if (attempt == maxAttempt)
                    {
                        throw new Exception($"Status Code: {response.StatusCode}");
                    }
                }
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> Post(HttpClient client, string path, string body)
        {
            var response
                = await client.PostAsync(path, new StringContent(body, Encoding.UTF8));

            if (!response.IsSuccessStatusCode)
            {                
                for (var attempt = 2; attempt <= maxAttempt; attempt++)
                {
                    await Task.Delay(100 * attempt);

                    response = await client.PostAsync(path, new StringContent(body, Encoding.UTF8));

                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }
                    else if (attempt == maxAttempt)
                    {
                        throw new Exception($"Status Code: {response.StatusCode}");
                    }
                }                
            }

            return await response.Content.ReadAsStringAsync();
        }

    }
}
