using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorContacts.Shared.Models;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using BlazorContacts.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData.Deltas;

namespace BlazorContacts.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ODataController
    {
        private readonly ContactsContext _context;

        public ContactsController (ContactsContext context)
        {
            _context = context;
        }

        // GET: api/contacts
        [EnableQuery(PageSize = 50)]
        public IQueryable<Contact> GetContacts()
        {
            return _context.Contacts;
        }

        // GET: api/contacts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContact(long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        // POST: api/contacts
        [HttpPost]
        public async Task<IActionResult> AddContact([FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Contacts.Add(contact);

            await _context.SaveChangesAsync();
            return Created(contact.Id.ToString(), contact);
        }

        // PUT: api/contacts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(long id, [FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != contact.Id)
            {
                return BadRequest();
            }

            try
            {
                var dbContact = await _context.Contacts.FindAsync(id);
                if (dbContact == null)
                {
                    return NotFound();
                }
                _context.Entry(dbContact).CurrentValues.SetValues(contact);
                await _context.SaveChangesAsync();
                return Updated(dbContact);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE: api/contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(long id)
        {
            var site = await _context.Contacts.FindAsync(id);
            if (site == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(site);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactExists(long id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}