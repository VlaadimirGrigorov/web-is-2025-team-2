using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using PhoneBook.Data;
using PhoneBook.DTOs;
using PhoneBook.Mappers;
using PhoneBook.Models;

namespace PhoneBook.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly PhoneBookContext _context;

        public ContactRepository(PhoneBookContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<List<ContactResponseDto>> GetContacts(int userId)
        {
            var contacts = await _context.Contacts.
                                Where(c => c.UserId == userId).
                                Include(c => c.PhoneNumbers).
                                ToListAsync();

            return contacts.Select(DtoMappers.ToDto).ToList();
        }
        public async Task<ContactResponseDto> GetContact(int userId, int contactId)
        {
            var contact = await _context.Contacts
                                .Where(c => c.UserId == userId && c.Id == contactId)
                                .Include(c => c.PhoneNumbers)
                                .FirstOrDefaultAsync();

            return contact == null ? null : DtoMappers.ToDto(contact);
        }
        public async Task<RepositoryResult<ContactResponseDto>> AddContact(int userId, ContactRequestDto contactDto)
        {
            var contact = DtoMappers.ToEntity(contactDto);
            contact.UserId = userId;

            if (contactDto.Name == null)
            {
                return RepositoryResult<ContactResponseDto>.Fail("Name field cannot be empty!");
            }

            if (contact.PhoneNumbers.Count == 0)
            {
                return RepositoryResult<ContactResponseDto>.Fail("Phone number field cannot be empty!");
            }

            // Check if a contact with the same name and userId, or same contactId and userId already exists
            var exisitingContact = await _context.Contacts.Where(c => c.UserId == userId && c.Name == contact.Name)
                                                          .Include(c => c.PhoneNumbers)
                                                          .FirstOrDefaultAsync();
            if (exisitingContact != null)
            {
                return RepositoryResult<ContactResponseDto>.Fail($"Cannot have contact with same name {exisitingContact.Name}.");
            }

            var contactExists = await _context.Contacts
                                              .Where(c => c.UserId == userId && c.Id == contact.Id)
                                              .Include(c => c.PhoneNumbers)
                                              .FirstOrDefaultAsync();
            if (contactExists != null)
            {
                return RepositoryResult<ContactResponseDto>.Fail($"Cannot have contact with same id {contactExists.Id}.");
            }

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return RepositoryResult<ContactResponseDto>.Ok(DtoMappers.ToDto(contact));
        }

        public async Task<RepositoryResult<PhoneNumberResponseDto>> AddPhoneToContact(int userId, int id, PhoneNumberRequestDto phoneNumberDto)
        {
            var contact = await _context.Contacts
                                        .Where(c => c.UserId == userId && c.Id == id)
                                        .Include(c => c.PhoneNumbers)
                                        .FirstOrDefaultAsync();
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

        public async Task<ContactResponseDto> DeleteContact(int userId, int id)
        {
            var contact = await _context.Contacts.
                                        Where(c => c.UserId == userId && c.Id == id).
                                        Include(c => c.PhoneNumbers).
                                        FirstOrDefaultAsync();
            if (contact == null)
            {
                return null;
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return DtoMappers.ToDto(contact);
        }

        public async Task<RepositoryResult<PhoneNumberResponseDto>> DeletePhoneNumberFromContact(int userId, int id, int phoneId)
        {
            var contact = await _context.Contacts
                                                .Where(c => c.UserId == userId && c.Id == id)
                                                .Include(c => c.PhoneNumbers)
                                                .FirstOrDefaultAsync();
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

        public async Task<ContactResponseDto> UpdateContact(int userId, int id, ContactRequestDto updatedContactDto)
        {
            var contact = await _context.Contacts
                                        .Where(c => c.UserId == userId && c.Id == id)
                                        .Include(c => c.PhoneNumbers)
                                        .FirstOrDefaultAsync();
            if (contact == null)
            {
                return null;
            }

            var updatedContact = DtoMappers.ToEntity(updatedContactDto);

            changeSpecificFields(contact, updatedContact);

            contact.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return DtoMappers.ToDto(contact);
        }

        public async Task<RepositoryResult<PhoneNumberResponseDto>> UpdatePhoneNumberInContact(int userId, int id, int phoneId, PhoneNumberRequestDto phoneNumberDto)
        {
            var contact = await _context.Contacts
                                        .Where(c => c.UserId == userId && c.Id == id)
                                        .Include(c => c.PhoneNumbers)
                                        .FirstOrDefaultAsync();
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

            contact.Address = updatedContact.Address ?? contact.Address;
        }
    }
}
