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

        public ApiService(HttpClient client)
        {
            _httpClient = client;
        }

        private async Task<string> requestNewToken()
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
                    Scope = "blazorcontacts-api",
                    ClientSecret = "thisismyclientspecificsecret",
                    Address = discovery.TokenEndpoint,
                    ClientId = "blazorcontacts-web"
                });

                if (tokenResponse.IsError)
                {
                    throw new ApplicationException($"Error: {tokenResponse.Error}");
                }

                return tokenResponse.AccessToken;
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }

        public async Task<List<Contact>> GetContactsAsync()
        {
            var access_token = await requestNewToken();
            _httpClient.SetBearerToken(access_token);

            var response = await _httpClient.GetAsync("api/contacts");
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<Contact>>(responseContent);
        }

        public async Task<Contact> GetContactByIdAsync(int id)
        {
            var access_token = await requestNewToken();
            _httpClient.SetBearerToken(access_token);

            var response = await _httpClient.GetAsync($"api/contacts/{id}");
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Contact>(responseContent);
        }
    }
}
