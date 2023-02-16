using BlazorContacts.Shared.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Net.Http.Json;

namespace BlazorContacts.Web.Services
{
    public class ApiService
    {
        public HttpClient _httpClient;

        public ApiService(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<ContactsApiResponse> GetContactsAsync(string orderBy, int skip, int top)
        {
            var response = await _httpClient.GetAsync($"api/contacts?$count=true&$orderby={orderBy}&$skip={skip}&$top={top}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ContactsApiResponse>(responseContent);
            }

            return new ContactsApiResponse();
        }

        public async Task<Contact> GetContactByIdAsync(long id)
        {
            var response = await _httpClient.GetAsync($"api/contacts/{id}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Contact>(responseContent);            
        }

        public async Task<HttpResponseMessage> DeleteContactByIdAsync(long id)
        {
            return await _httpClient.DeleteAsync($"api/contacts/{id}");
        }

        public async Task<HttpResponseMessage> CreateContactAsync(Contact contact)
        {
            return await _httpClient.PostAsJsonAsync($"api/contacts", contact);
        }

        public async Task<HttpResponseMessage> EditContactAsync(long id, Contact contact)
        {
            return await _httpClient.PutAsJsonAsync($"api/contacts/{id}", contact);
        }
    }
}