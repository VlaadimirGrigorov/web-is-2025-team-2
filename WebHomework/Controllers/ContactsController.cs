using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebHomework.Data;
using WebHomework.Models;

namespace WebHomework.Controllers
{
    [Route("api/contacts")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly PhoneBookContext _context;

        public ContactsController(PhoneBookContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Contact>>> GetContacts()
        {
            return await _context.Contacts.Include(c => c.PhoneNumbers).Include(c => c.Address).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            var contact = await _context.Contacts
                                .Include(c => c.Address)
                                .Include(c => c.PhoneNumbers)
                                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        [HttpPost]
        public async Task<ActionResult<Contact>> AddContact(Contact contact)
        {
            var existingContact = await _context.Contacts.FindAsync(contact.Id);
            if (existingContact != null)
            {
                return Conflict("Contact already exists!");
            }

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Contact>> DeleteContact(int id)
        {
            var existingContact = await _context.Contacts.Include(c => c.Address).Include(c => c.PhoneNumbers).FirstOrDefaultAsync(c => c.Id == id);
            if (existingContact == null)
            {
                return NotFound($"Contact with id {id} doesn't exist!");
            }

            _context.Contacts.Remove(existingContact);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact([FromRoute] int id, [FromBody] Contact updatedContact)
        {
            var contact = await _context.Contacts.Include(c => c.PhoneNumbers).Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            changeSpecificFields(contact, updatedContact);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void changeSpecificFields(Contact contact, Contact updatedContact)
        {
            if (updatedContact.Name != null)
                contact.Name = updatedContact.Name;
            if (updatedContact.Address != null)
            {
                if (contact.Address == null)
                {
                    contact.Address = new Address();
                }
                contact.Address.Street = updatedContact.Address.Street;
                contact.Address.City = updatedContact.Address.City;
                contact.Address.PostalCode = updatedContact.Address.PostalCode;
            }

            if (updatedContact.PhoneNumbers != null)
            {
                if (contact.PhoneNumbers == null)
                {
                    contact.PhoneNumbers = new List<PhoneNumber>();
                }
                contact.PhoneNumbers = updatedContact.PhoneNumbers;
            }
        }
    }
}
