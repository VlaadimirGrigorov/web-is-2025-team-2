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
            foreach(var phoneNumber in contact.PhoneNumbers)
            {
                if (phoneNumber.Number == null || !isValidPhoneNumber(phoneNumber.Number))
                {
                    return BadRequest("Invalid phone number!");
                }
            }

            var existingContact = await _context.Contacts.FindAsync(contact.Id);
            if (existingContact != null)
            {
                return Conflict("Contact already exists!");
            }

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
        }

        [HttpPost("/{id}/phonenumbers")]
        public async Task<ActionResult<Contact>> AddPhoneToContact(int id, PhoneNumber phoneNumber)
        {
            if (phoneNumber.Number == null || !isValidPhoneNumber(phoneNumber.Number))
            {
                return BadRequest($"Invalid phone number!");
            }

            var contact = await _context.Contacts.Include(c => c.PhoneNumbers).Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
            {
                return NotFound($"Contact with id {id} was not found!");
            }

            var exist = await _context.PhoneNumbers.FirstOrDefaultAsync(c => c.Number == phoneNumber.Number);
            if (exist != null)
            {
                return Conflict($"Phone number {phoneNumber.Number} already exists!");
            }

            contact.PhoneNumbers.Add(phoneNumber);
            
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, phoneNumber);
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

        [HttpDelete("/{id}/phonenumbers/{phoneId}")]
        public async Task<ActionResult<Contact>> DeletePhoneNumberFromContact(int id, int phoneId)
        {
            var contact = await _context.Contacts.Include(c => c.PhoneNumbers).Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
            {
                return NotFound($"Contact with id {id} was not found!");
            }

            var phoneNumber = await _context.PhoneNumbers.FirstOrDefaultAsync(c => c.Id == phoneId);
            if (phoneNumber == null)
            {
                return NotFound($"Phone number with id {phoneId} was not found!");
            }

            if (!contact.PhoneNumbers.Any(c => c.Id == phoneId))
            {
                return BadRequest($"Phone number with id {phoneId} does not belong to contact with id {id}.");
            }

            contact.PhoneNumbers.Remove(phoneNumber);

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

        [HttpPatch("{id}/phonenumbers/{phoneId}")]
        public async Task<IActionResult> UpdatePhoneNumberInContact(int id, int phoneId, PhoneNumber phoneNumber)
        {
            var contact = await _context.Contacts.Include(c => c.PhoneNumbers).Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
            {
                return NotFound($"Contact with id {id} was not found!");
            }

            var number = await _context.PhoneNumbers.FirstOrDefaultAsync(c => c.Id == phoneId);
            if (number == null)
            {
                return NotFound($"Phone with id {phoneId} was not found!");
            }

            if (!contact.PhoneNumbers.Any(p => p.Id == phoneId))
            {
                return BadRequest($"Phone number with id {phoneId} does not belong to contact with id {id}.");
            }

            var phone = contact.PhoneNumbers.FirstOrDefault(c => c.Id == phoneId);
            if (phone != null)
            {
                phone.Number = phoneNumber.Number;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void changeSpecificFields(Contact contact, Contact updatedContact)
        {
            contact.Name = updatedContact.Name ?? contact.Name;
            
            if (updatedContact.Address != null)
            {
                if (contact.Address == null)
                {
                    contact.Address = new Address();
                }
                contact.Address.Street = updatedContact.Address.Street ?? contact.Address.Street;
                contact.Address.City = updatedContact.Address.City ?? contact.Address.City;
                contact.Address.PostalCode = updatedContact.Address.PostalCode ?? contact.Address.PostalCode;
            }

            if (updatedContact.PhoneNumbers != null)
            {
                contact.PhoneNumbers.Clear();
                foreach (var phone in updatedContact.PhoneNumbers)
                {
                    contact.PhoneNumbers.Add(phone);
                }
            }

            contact.UpdatedAt = DateTime.Now;
        }

        private bool isValidPhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length > 10)
                return false;

            return phoneNumber.All(char.IsDigit);
        }
    }
}
