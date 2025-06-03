using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebHomework.Data;
using WebHomework.DTOs;
using WebHomework.Mappers;
using WebHomework.Models;

namespace WebHomework.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly PhoneBookContext _context;

        public ContactRepository(PhoneBookContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<List<ContactResponseDto>> GetContacts()
        {
            var contacts = await _context.Contacts.Include(c => c.PhoneNumbers).ToListAsync();

            return contacts.Select(DtoMappers.ToDto).ToList();
        }

        public async Task<ContactResponseDto> GetContact(int id)
        {
            var contact = await _context.Contacts
                                .Include(c => c.PhoneNumbers)
                                .FirstOrDefaultAsync(c => c.Id == id);

            return contact == null ? null : DtoMappers.ToDto(contact);
        }

        public async Task<ContactResponseDto> AddContact(ContactRequestDto contactDto)
        {
            var contact = DtoMappers.ToEntity(contactDto);

            var contactExists = await _context.Contacts.Include(c => c.PhoneNumbers).FirstOrDefaultAsync(c => c.Id == contact.Id);
            if (contactExists != null)
            {
                return null;
            }

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return DtoMappers.ToDto(contact);
        }

        public async Task<RepositoryResult<PhoneNumberResponseDto>> AddPhoneToContact(int id, PhoneNumberRequestDto phoneNumberDto)
        {
            var contact = await _context.Contacts.Include(c => c.PhoneNumbers).FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
            {
                return RepositoryResult<PhoneNumberResponseDto>.Fail($"Contact with id {id} was not found!");
            }

            var exist = await _context.PhoneNumbers.FirstOrDefaultAsync(c => c.Number == phoneNumberDto.Number);
            if (exist != null)
            {
                return RepositoryResult<PhoneNumberResponseDto>.Fail($"Phone number {phoneNumberDto.Number} already exists!");
            }

            var phoneNumber = DtoMappers.ToEntity(phoneNumberDto);

            contact.PhoneNumbers.Add(phoneNumber);

            await _context.SaveChangesAsync();

            return RepositoryResult<PhoneNumberResponseDto>.Ok(DtoMappers.ToDto(phoneNumber));
        }

        public async Task<ContactResponseDto> DeleteContact(int id)
        {
            var contact = await _context.Contacts.Include(c => c.PhoneNumbers).FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
            {
                return null;
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return DtoMappers.ToDto(contact);
        }

        public async Task<RepositoryResult<PhoneNumberResponseDto>> DeletePhoneNumberFromContact(int id, int phoneId)
        {
            var contact = await _context.Contacts.Include(c => c.PhoneNumbers).Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
            {
                return RepositoryResult<PhoneNumberResponseDto>.Fail($"Contact with id {id} was not found!");
            }

            var phoneNumber = await _context.PhoneNumbers.FirstOrDefaultAsync(c => c.Id == phoneId);
            if (phoneNumber == null)
            {
                return RepositoryResult<PhoneNumberResponseDto>.Fail($"Phone number with id {phoneId} was not found!");
            }

            if (!contact.PhoneNumbers.Any(c => c.Id == phoneId))
            {
                return RepositoryResult<PhoneNumberResponseDto>.Fail($"Phone number with id {phoneId} does not belong to contact with id {id}.");
            }

            contact.PhoneNumbers.Remove(phoneNumber);

            await _context.SaveChangesAsync();

            return RepositoryResult<PhoneNumberResponseDto>.Ok(DtoMappers.ToDto(phoneNumber));
        }

        public async Task<ContactResponseDto> UpdateContact([FromRoute] int id, [FromBody] ContactRequestDto updatedContactDto)
        {
            var contact = await _context.Contacts.Include(c => c.PhoneNumbers).Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
            {
                return null;
            }

            var updatedContact = DtoMappers.ToEntity(updatedContactDto);

            changeSpecificFields(contact, updatedContact);

            await _context.SaveChangesAsync();

            return DtoMappers.ToDto(contact);
        }

        public async Task<RepositoryResult<PhoneNumberResponseDto>> UpdatePhoneNumberInContact(int id, int phoneId, PhoneNumberRequestDto phoneNumberDto)
        {
            var contact = await _context.Contacts.Include(c => c.PhoneNumbers).Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
            {
                return RepositoryResult<PhoneNumberResponseDto>.Fail($"Contact with id {id} was not found!");
            }

            var number = await _context.PhoneNumbers.FirstOrDefaultAsync(c => c.Id == phoneId);
            if (number == null)
            {
                return RepositoryResult<PhoneNumberResponseDto>.Fail($"Phone with id {phoneId} was not found!");
            }

            if (!contact.PhoneNumbers.Any(p => p.Id == phoneId))
            {
                return RepositoryResult<PhoneNumberResponseDto>.Fail($"Phone number with id {phoneId} does not belong to contact with id {id}.");
            }

            var phone = contact.PhoneNumbers.FirstOrDefault(c => c.Id == phoneId);
            phone.Number = phoneNumberDto.Number;

            await _context.SaveChangesAsync();

            return RepositoryResult<PhoneNumberResponseDto>.Ok(DtoMappers.ToDto(phone));
        }

        private void changeSpecificFields(Contact contact, Contact updatedContact)
        {
            contact.Name = updatedContact.Name ?? contact.Name;

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
    }
}
