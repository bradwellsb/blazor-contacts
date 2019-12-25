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

            //Handle more gracefully
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Contact>(responseContent);
        }

        public async Task<HttpResponseMessage> DeleteContactByIdAsync(long id)
        {
            //consider impact vs returning just status code
            return await _httpClient.DeleteAsync($"api/contacts/{id}");
        }

        public async Task<HttpResponseMessage> CreateContactAsync(Contact contact)
        {
            string jsonContact = JsonSerializer.Serialize(contact);
            var stringContent = new StringContent(jsonContact, System.Text.Encoding.UTF8, "application/json");

            return await _httpClient.PostAsync($"api/contacts", stringContent);
        }

        public async Task<HttpResponseMessage> EditContactAsync(long id, Contact contact)
        {
            string jsonContact = JsonSerializer.Serialize(contact);
            var stringContent = new StringContent(jsonContact, System.Text.Encoding.UTF8, "application/json");

            return await _httpClient.PatchAsync($"api/contacts/{id}", stringContent);
        }
    }
}