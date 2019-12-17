using IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorContacts.Web.Services
{
    public class ApiTokenCacheService
    {
        private readonly HttpClient _httpClient;

        private static readonly Object _lock = new Object();
        private IDistributedCache _cache;

        private const int cacheExpirationInDays = 1;

        private class AccessTokenItem
        {
            public string AccessToken { get; set; } = string.Empty;
            public DateTime Expiry { get; set; }
        }

        public ApiTokenCacheService(
            IHttpClientFactory httpClientFactory,
            IDistributedCache cache)
        {
            _httpClient = httpClientFactory.CreateClient();
            _cache = cache;
        }

        public async Task<string> GetAccessToken(string client_name, string api_scope, string secret)
        {
            AccessTokenItem accessToken = GetFromCache(client_name);

            if (accessToken != null && accessToken.Expiry > DateTime.UtcNow)
            {
                return accessToken.AccessToken;
            }

            // Token not cached, or token is expired. Request new token from auth server
            AccessTokenItem newAccessToken = await RequestNewToken(client_name, api_scope, secret);
            AddToCache(client_name, newAccessToken);

            return newAccessToken.AccessToken;
        }

        private AccessTokenItem GetFromCache(string key)
        {
            var item = _cache.GetString(key);
            if (item != null)
            {
                return JsonSerializer.Deserialize<AccessTokenItem>(item);
            }

            return null;
        }

        private void AddToCache(string key, AccessTokenItem accessTokenItem)
        {
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(cacheExpirationInDays));

            lock (_lock)
            {
                _cache.SetString(key, JsonSerializer.Serialize(accessTokenItem), options);
            }
        }

        private async Task<AccessTokenItem> RequestNewToken(string client_name, string api_scope, string secret)
        {
            try
            {
                var discovery = await HttpClientDiscoveryExtensions.GetDiscoveryDocumentAsync(
                    _httpClient, "http://localhost:5000");

                if (discovery.IsError)
                {
                    throw new ApplicationException($"Error: {discovery.Error}");
                }

                var tokenResponse = await HttpClientTokenRequestExtensions.RequestClientCredentialsTokenAsync(_httpClient, new ClientCredentialsTokenRequest
                {
                    Scope = api_scope,
                    ClientSecret = secret,
                    Address = discovery.TokenEndpoint,
                    ClientId = client_name
                });

                if (tokenResponse.IsError)
                {
                    throw new ApplicationException($"Error: {tokenResponse.Error}");
                }

                return new AccessTokenItem
                {
                    Expiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    AccessToken = tokenResponse.AccessToken
                };
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }
    }
}
