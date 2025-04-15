using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebHomework.Data;
using WebHomework.DTOs;
using WebHomework.Models;
using WebHomework.Mappers;

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
        public async Task<ActionResult<List<ContactResponseDto>>> GetContacts()
        {
            var contacts = await _context.Contacts.Include(c => c.PhoneNumbers).Include(c => c.Address).ToListAsync();

            return contacts.Select(DtoMappers.ToDto).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactResponseDto>> GetContact(int id)
        {
            var contact = await _context.Contacts
                                .Include(c => c.Address)
                                .Include(c => c.PhoneNumbers)
                                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            var response = DtoMappers.ToDto(contact);

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ContactResponseDto>> AddContact(ContactRequestDto contactDto)
        {
            foreach(var phoneNumber in contactDto.PhoneNumbers)
            {
                if (!IsValidPhoneNumber(phoneNumber.Number))
                {
                    return BadRequest("Invalid phone number!");
                }
            }

            var contact = DtoMappers.ToEntity(contactDto);

            var contactExists = await _context.Contacts.Include(c => c.Address).Include(c => c.PhoneNumbers).FirstOrDefaultAsync(c => c.Id == contact.Id);
            if (contactExists != null)
            {
                return Conflict("Contact already exists!");
            }

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            var response = DtoMappers.ToDto(contact);

            return CreatedAtAction(nameof(GetContact), new { id = response.Id }, response);
        }

        [HttpPost("/{id}/phonenumbers")]
        public async Task<ActionResult<PhoneNumberResponseDto>> AddPhoneToContact(int id, PhoneNumberRequestDto phoneNumberDto)
        {
            if (!IsValidPhoneNumber(phoneNumberDto.Number))
            {
                return BadRequest($"Invalid phone number!");
            }

            var contact = await _context.Contacts.Include(c => c.PhoneNumbers).Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
            {
                return NotFound($"Contact with id {id} was not found!");
            }

            var exist = await _context.PhoneNumbers.FirstOrDefaultAsync(c => c.Number == phoneNumberDto.Number);
            if (exist != null)
            {
                return Conflict($"Phone number {phoneNumberDto.Number} already exists!");
            }

            var phoneNumber = DtoMappers.ToEntity(phoneNumberDto);

            contact.PhoneNumbers.Add(phoneNumber);
            
            await _context.SaveChangesAsync();

            var response = DtoMappers.ToDto(phoneNumber);

            return CreatedAtAction(nameof(GetContact), new { id = response.Id }, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ContactResponseDto>> DeleteContact(int id)
        {
            var contact = await _context.Contacts.Include(c => c.Address).Include(c => c.PhoneNumbers).FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
            {
                return NotFound($"Contact with id {id} doesn't exist!");
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            var response = DtoMappers.ToDto(contact);

            return Ok(response);
        }

        [HttpDelete("/{id}/phonenumbers/{phoneId}")]
        public async Task<ActionResult<PhoneNumberResponseDto>> DeletePhoneNumberFromContact(int id, int phoneId)
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

            var response = DtoMappers.ToDto(phoneNumber);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ContactResponseDto>> UpdateContact([FromRoute] int id, [FromBody] ContactRequestDto updatedContactDto)
        {
            var contact = await _context.Contacts.Include(c => c.PhoneNumbers).Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            var updatedContact = DtoMappers.ToEntity(updatedContactDto);

            changeSpecificFields(contact, updatedContact);

            await _context.SaveChangesAsync();

            var response = DtoMappers.ToDto(contact);

            return Ok(response);
        }

        [HttpPatch("{id}/phonenumbers/{phoneId}")]
        public async Task<ActionResult<PhoneNumberResponseDto>> UpdatePhoneNumberInContact(int id, int phoneId, PhoneNumberRequestDto phoneNumberDto)
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
            phone.Number = phoneNumberDto.Number;

            await _context.SaveChangesAsync();

            var response = DtoMappers.ToDto(phone);

            return Ok(response);
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

            var updatedNumbers = updatedContact.PhoneNumbers.Select(p => p.Number).ToList();
            var currentNumbers = contact.PhoneNumbers.Select(p => p.Number).ToList();

            if (!updatedNumbers.SequenceEqual(currentNumbers))
            {
                contact.PhoneNumbers.Clear();
                foreach (var phone in updatedContact.PhoneNumbers)
                {
                    contact.PhoneNumbers.Add(phone);
                }
            }

            contact.UpdatedAt = DateTime.Now;
        }

        private bool IsValidPhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length > 10)
                return false;

            return phoneNumber.All(char.IsDigit);
        }
    }
}
