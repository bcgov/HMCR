using Hmcr.Model;
using Hmcr.Model.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IKeyCloakService
    {
        Task<(bool Valid, bool NotFound)> VerifyUserIdAsync(string userId);
        Task<(string Error, string Password)> ResetUserPasswordAsync(string userId);
    }

    public class KeyCloakService : IKeyCloakService
    {
        private IConfiguration _config;
        private IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient;
        private HmcrCurrentUser _currentUser;
        private ILogger<KeyCloakService> _logger;

        private string _serviceClientId;
        private string _serviceClientSecret;
        private string _authHost;

        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public KeyCloakService(IConfiguration config, IHttpClientFactory httpClientFactory, HmcrCurrentUser currentUser, ILogger<KeyCloakService> logger)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
            _currentUser = currentUser;
            _logger = logger;

            _serviceClientId = config.GetValue<string>("KeyCloak:ServiceClientId");
            _serviceClientSecret = config.GetValue<string>("KeyCloak:ServiceClientSecret");
            _authHost = config.GetValue<string>("JWT:Authority");
        }

        public async Task<(bool Valid, bool NotFound)> VerifyUserIdAsync(string userId)
        {
            try
            {
                if (_httpClient == null)
                    _httpClient = await CreateHttpClientWithTokenAsync();

                var response = await _httpClient.GetAsync(userId);


                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var keycloakUser = JsonSerializer.Deserialize<KeyCloakUserResponse>(content, _jsonOptions);

                    if (keycloakUser.Email == _currentUser.Email.ToLowerInvariant())
                    {
                        return (true, false);
                    }

                    return (false, false);
                }
                else
                {
                    return (false, true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - VerifyUserIdAsync: {ex}");
                throw ex;
            }
        }

        public async Task<(string Error, string Password)> ResetUserPasswordAsync(string userId)
        {
            try
            {
                if (_httpClient == null)
                    _httpClient = await CreateHttpClientWithTokenAsync();

                var password = GetUniqueToken(32);

                var payload = JsonSerializer.Serialize(new KeyCloakUserPasswordResetRequest(password), _jsonOptions);
                var response = await _httpClient.PutAsync($"{userId}/reset-password", new StringContent(payload, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return ("", password);
                }
                else
                {
                    return (response.ReasonPhrase, "");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - ResetUserPasswordAsync: {ex}");
                throw ex;
            }
        }

        private async Task<HttpClient> CreateHttpClientWithTokenAsync()
        {
            var content = "";
            var basicAuth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{_serviceClientId}:{_serviceClientSecret}"));
            var requestToken = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_authHost}/protocol/openid-connect/token"),
                Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" } })
            };
            requestToken.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.SendAsync(requestToken);

                content = await response.Content.ReadAsStringAsync();
                var keyCloakToken = JsonSerializer.Deserialize<KeyCloakTokenResponse>(content);

                return BuildHttpClient(keyCloakToken.AccessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - CreateHttpClientWithTokenAsync: {ex}");
                throw ex;
            }
        }

        private HttpClient BuildHttpClient(string token)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.BaseAddress = new Uri($"{_authHost.Replace("auth/realms", "auth/admin/realms")}/users/");

            return httpClient;
        }

        // https://blog.bitscry.com/2018/04/13/cryptographically-secure-random-string/
        private string GetUniqueToken(int length, string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_!@#$%^&*")
        {
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[length];

                // If chars.Length isn't a power of 2 then there is a bias if we simply use the modulus operator. The first characters of chars will be more probable than the last ones.
                // buffer used if we encounter an unusable random byte. We will regenerate it in this buffer
                byte[] buffer = null;

                // Maximum random number that can be used without introducing a bias
                int maxRandom = byte.MaxValue - ((byte.MaxValue + 1) % chars.Length);

                crypto.GetBytes(data);

                char[] result = new char[length];

                for (int i = 0; i < length; i++)
                {
                    byte value = data[i];

                    while (value > maxRandom)
                    {
                        if (buffer == null)
                        {
                            buffer = new byte[1];
                        }

                        crypto.GetBytes(buffer);
                        value = buffer[0];
                    }

                    result[i] = chars[value % chars.Length];
                }

                return new string(result);
            }
        }
    }
}
