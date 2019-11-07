using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public async Task<string> Get(HttpClient client, string path)
        {
            var response = await client.GetAsync(path);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Status Code: {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> Post(HttpClient client, string path, string body)
        {
            var response
                = await client.PostAsync(path, new StringContent(body, Encoding.UTF8));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Status Code: {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }

    }
}
