using IdentityModel.Client;
using BlazorContacts.Shared.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace BlazorContacts.Web.Services
{

    public class ApiService
    {
        public HttpClient _httpClient;
        private readonly ApiTokenCacheService _apiTokenService;

        public ApiService(HttpClient client, ApiTokenCacheService apiTokenCacheService)
        {
            _httpClient = client;
            _apiTokenService = apiTokenCacheService;
        }

        public async Task<List<Contact>> GetContactsAsync()
        {
            var access_token = await _apiTokenService.GetAccessToken(
                "blazorcontacts-web",
                "blazorcontacts-api",
                "thisismyclientspecificsecret"
            );
            _httpClient.SetBearerToken(access_token);

            var response = await _httpClient.GetAsync("api/contacts");
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<Contact>>(responseContent);
        }

        public async Task<Contact> GetContactByIdAsync(int id)
        {
            var access_token = await _apiTokenService.GetAccessToken(
                "blazorcontacts-web",
                "blazorcontacts-api",
                "thisismyclientspecificsecret"
            );
            _httpClient.SetBearerToken(access_token);

            var response = await _httpClient.GetAsync($"api/contacts/{id}");
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Contact>(responseContent);
        }
    }
}