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
        public async Task<List<Contact>> GetContactsAsync()
        {
            var response = await _httpClient.GetAsync("api/contacts");
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<Contact>>(responseContent);
        }

        public async Task<Contact> GetContactByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/contacts/{id}");
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Contact>(responseContent);
        }
    }
}
