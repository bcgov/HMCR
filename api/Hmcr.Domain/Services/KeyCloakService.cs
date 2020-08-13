using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos.Keycloak;
using Hmcr.Model.Dtos.User;
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
    public interface IKeycloakService
    {
        Task<KeycloakClientDto> GetUserClientAsync();
        Task<(Dictionary<string, List<string>> Errors, KeycloakClientDto Client)> CreateUserClientAsync();
        Task<(bool NotFound, string Error)> RegenerateUserClientSecretAsync();
    }

    public class KeyCloakService : IKeycloakService
    {
        private IConfiguration _config;
        private IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient;
        private HmcrCurrentUser _currentUser;
        private ILogger<KeyCloakService> _logger;
        private IUserRepository _userRepo;
        private IUnitOfWork _unitOfWork;

        private string _serviceClientId;
        private string _serviceClientSecret;
        private string _authority;
        private string _audience;
        private string _realm;

        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true };

        public KeyCloakService(IConfiguration config, IHttpClientFactory httpClientFactory, HmcrCurrentUser currentUser, ILogger<KeyCloakService> logger, IUserRepository userRepo, IUnitOfWork unitOfWork)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
            _currentUser = currentUser;
            _logger = logger;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;

            _serviceClientId = config.GetValue<string>("Keycloak:ServiceClientId");
            _serviceClientSecret = config.GetValue<string>("Keycloak:ServiceClientSecret");
            _authority = config.GetValue<string>("JWT:Authority");
            _audience = config.GetValue<string>("JWT:Audience");

            var authorityUri = new Uri(_authority);
            _realm = authorityUri.Segments[authorityUri.Segments.Length - 1];
        }

        public async Task<KeycloakClientDto> GetUserClientAsync()
        {
            try
            {
                if (_httpClient == null)
                    _httpClient = await CreateHttpClientWithTokenAsync();

                var clientId = _currentUser.ApiClientId;
                KeycloakClientDto keycloakClient = null;

                if (!string.IsNullOrEmpty(clientId))
                {
                    var response = await _httpClient.GetAsync($"clients/{clientId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        keycloakClient = JsonSerializer.Deserialize<KeycloakClientDto>(content, _jsonOptions);
                    }

                    if (keycloakClient != null)
                    {
                        response = await _httpClient.GetAsync($"clients/{clientId}/client-secret");
                        var content = await response.Content.ReadAsStringAsync();
                        var keycloakSecret = JsonSerializer.Deserialize<KeycloakClientSecretDto>(content, _jsonOptions);
                        keycloakClient.ClientSecret = keycloakSecret.Value;
                    }
                }

                return keycloakClient;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - GetUserClientAsync: {ex}");
                throw ex;
            }
        }
        public async Task<(Dictionary<string, List<string>> Errors, KeycloakClientDto Client)> CreateUserClientAsync()
        {
            try
            {
                var errors = new Dictionary<string, List<string>>();

                if (_httpClient == null)
                    _httpClient = await CreateHttpClientWithTokenAsync();

                // Verify if user already has a Keycloak client
                var existingClient = await GetUserClientAsync();
                if (existingClient != null)
                {
                    errors.AddItem(Fields.ApiClientId, "Api client already exists for this user.");
                }

                if (errors.Count > 0)
                    return (errors, existingClient);


                var directoryType = _currentUser.UserType == UserTypeDto.INTERNAL ? "idir" : "bceid";
                var username = $"{_currentUser.Username.ToLowerInvariant()}@{directoryType}";
                var email = _currentUser.Email.ToLowerInvariant();
                var guidClaimValue = _currentUser.UserGuid.ToString();

                var createDto = new KeycloakClientCreateDto(_audience, username, email, $"{directoryType}_userid", guidClaimValue);
                createDto.ClientId = $"api.{_currentUser.Username.ToLowerInvariant()}.{GetUniqueToken(3)}";

                var payload = JsonSerializer.Serialize(createDto, _jsonOptions);
                var response = await _httpClient.PostAsync("clients", new StringContent(payload, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                    errors.AddItem(Fields.ApiClientId, response.ReasonPhrase);

                // Get newly created Keycloak client INTERNAL id
                var newId = response.Headers.Location.Segments[response.Headers.Location.Segments.Length - 1];
                _currentUser.ApiClientId = newId;

                // Write new Keycloak client INTERNAL id to database 
                await _userRepo.UpdateUserApiClientId(newId);
                _unitOfWork.Commit();

                existingClient = await GetUserClientAsync();

                return (errors, existingClient);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - CreateUserClientAsync: {ex}");
                throw ex;
            }
        }

        public async Task<(bool NotFound, string Error)> RegenerateUserClientSecretAsync()
        {
            try
            {
                if (_httpClient == null)
                    _httpClient = await CreateHttpClientWithTokenAsync();

                var clientId = _currentUser.ApiClientId;
                var notFound = false;
                var error = "";

                var existingClient = await GetUserClientAsync();
                if (existingClient == null)
                {
                    notFound = true;
                }
                else
                {
                    var payload = JsonSerializer.Serialize(new { Realm = _realm, Client = clientId }, _jsonOptions);
                    var response = await _httpClient.PostAsync($"clients/{clientId}/client-secret", new StringContent(payload, Encoding.UTF8, "application/json"));
                    if (!response.IsSuccessStatusCode)
                    {
                        error = response.ReasonPhrase;
                    }
                }

                return (notFound, error);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception - RegenerateUserClientSecretAsync: {ex}");
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
                RequestUri = new Uri($"{_authority}/protocol/openid-connect/token"),
                Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" } })
            };
            requestToken.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.SendAsync(requestToken);

                content = await response.Content.ReadAsStringAsync();
                var keyCloakToken = JsonSerializer.Deserialize<KeycloakTokenDto>(content);

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
            httpClient.BaseAddress = new Uri($"{_authority.Replace("auth/realms", "auth/admin/realms")}/");

            return httpClient;
        }

        // https://blog.bitscry.com/2018/04/13/cryptographically-secure-random-string/
        private string GetUniqueToken(int length, string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890")
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
