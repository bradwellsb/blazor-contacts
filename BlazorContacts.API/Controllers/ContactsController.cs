using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorContacts.Shared.Models;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData;
using BlazorContacts.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BlazorContacts.API.Controllers
{
    [Authorize]
    //[Route("api/[controller]")]
    [ODataRoutePrefix("contacts")]
    //[ApiController]
    public class ContactsController : ODataController
    {
        private readonly ContactsContext _context;

        public ContactsController (ContactsContext context)
        {
            _context = context;
        }

        // GET: api/contacts
        [EnableQuery(PageSize = 50)]
        [ODataRoute]
        public IQueryable<Contact> Get()
        {
            return _context.Contacts;
        }

        // GET: api/contacts(5)
        [EnableQuery]
        [ODataRoute("({id})")]
        public async Task<IActionResult> GetContact([FromODataUri] long id)
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
        [ODataRoute]
        public async Task<IActionResult> Post([FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Contacts.Add(contact);

            await _context.SaveChangesAsync();
            return Created(contact.Id.ToString(), contact);
        }

        // PATCH: api/contacts(5)
        [ODataRoute("({id})")]
        public async Task<IActionResult> Patch([FromODataUri] long id, [FromBody] Delta<Contact> updatedContact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbContact = await _context.Contacts.FindAsync(id);
            if (dbContact == null)
            {
                return NotFound();
            }
            updatedContact.Patch(dbContact);
            try
            {
                await _context.SaveChangesAsync();
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
            return Updated(dbContact);
        }

        // DELETE: api/contacts(5)
        [ODataRoute("({id})")]
        public async Task<IActionResult> Delete([FromODataUri] long id)
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