using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorContacts.Shared.Models;

namespace BlazorContacts.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private static readonly List<Contact> contacts = GenerateContacts(5);
        private static List<Contact> GenerateContacts(int number)
        {
            return Enumerable.Range(1, number).Select(index => new Contact
            {
                Id = index,
                Name = $"First{index} Last{index}",
                PhoneNumber = $"+1 555 {index}{index}{index} 987{index}",
            }).ToList();
        }

        // GET: api/contacts
        [HttpGet]
        public ActionResult<List<Contact>> GetAllContacts()
        {
            return contacts;
        }

        // GET: api/contacts/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Contact> GetContactById(int id)
        {
            var contact = contacts.FirstOrDefault((p) => p.Id == id);
            if (contact == null)
                return NotFound();
            return contact;
        }

        // POST: api/contacts
        [HttpPost]
        public void AddContact([FromBody] Contact contact)
        {
            contacts.Add(contact);
        }

        // PUT: api/contacts/5
        [HttpPut("{id}")]
        public void EditContact(int id, [FromBody] Contact contact)
        {
            int index = contacts.FindIndex((p) => p.Id == id);
            if (index != -1)
                contacts[index] = contact;

        }

        // DELETE: api/contacts/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            int index = contacts.FindIndex((p) => p.Id == id);
            if (index != -1)
                contacts.RemoveAt(index);
        }
    }
}