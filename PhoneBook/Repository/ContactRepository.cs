using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBook.Data;
using PhoneBook.DTOs;
using PhoneBook.Mappers;
using PhoneBook.Models;
using FileServ.Controllers.Helpers;

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
                                Include(c => c.Photo).
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
            var contact = await _context.Contacts
                                        .Where(c => c.UserId == userId && c.Id == id)
                                        .Include(c => c.PhoneNumbers)
                                        .Include(c => c.Photo) // Eagerly load the Photo
                                        .FirstOrDefaultAsync();
            if (contact == null)
            {
                return null;
            }

            // Handle photo deletion if a photo exists
            if (contact.Photo != null)
            {
                // 1. Delete the physical file
                var uploadHandler = new UploadHandler(); // Helper to manage file operations
                if (!string.IsNullOrEmpty(contact.Photo.FilePath))
                {
                    await uploadHandler.Remove(contact.Photo.FilePath);
                }

                // 2. Remove the Photo entity from the context
                _context.Photos.Remove(contact.Photo);
            }

            // PhoneNumbers are typically configured for cascade delete by convention if the FK is non-nullable,
            // or they will be orphaned if the FK is nullable and not handled.
            // If PhoneNumbers need explicit deletion and are not cascaded, they should be removed here.
            // For now, assuming cascade delete or existing handling is sufficient for PhoneNumbers based on the original code.
            // If PhoneNumbers are not set for cascade delete and ContactId is nullable, you might do:
            // _context.PhoneNumbers.RemoveRange(contact.PhoneNumbers); 
            // Or if ContactId is nullable and you want to orphan them:
            // foreach (var pn in contact.PhoneNumbers) { pn.ContactId = null; }

            // Remove the Contact entity itself
            _context.Contacts.Remove(contact);

            // Save all changes to the database
            await _context.SaveChangesAsync();

            // DtoMappers.ToDto might fail if contact.Photo was just deleted and it tries to access it.
            // It's safer to map before deletion or construct a simple DTO here.
            // However, the original method returned the DTO of the deleted contact.
            // Let's stick to that pattern but be mindful. If ToDto tries to access contact.Photo.FilePath it will be null.
            // The current DtoMappers.ToDto checks for contact.Photo != null, so it should be fine.
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

        public async Task<ContactResponseDto> AddPhotoToContact(int userId, int contactId, string filePath)
        {
            var contact = await _context.Contacts
                                        .Where(c => c.UserId == userId && c.Id == contactId)
                                        .Include(c => c.Photo)
                                        .FirstOrDefaultAsync();

            if (contact == null)
            {
                return null; // Or throw an exception
            }

            if (contact.Photo != null)
            {
                // Create an instance of UploadHandler to call the Remove method
                var uploadHandler = new UploadHandler();
                uploadHandler.Remove(contact.Photo.FilePath);
                _context.Photos.Remove(contact.Photo);
            }

            var newPhoto = new Photo { ContactId = contactId, FilePath = filePath };
            contact.Photo = newPhoto;

            await _context.SaveChangesAsync();

            return DtoMappers.ToDto(contact);
        }

        public async Task<bool> DeletePhotoFromContact(int userId, int contactId)
        {
            var contact = await _context.Contacts
                                        .Where(c => c.UserId == userId && c.Id == contactId)
                                        .Include(c => c.Photo)
                                        .FirstOrDefaultAsync();

            if (contact == null || contact.Photo == null)
            {
                return false; // Contact or photo not found
            }

            var filePath = contact.Photo.FilePath;

            _context.Photos.Remove(contact.Photo);

            // Create an instance of UploadHandler to call the Remove method
            var uploadHandler = new UploadHandler();
            if (!string.IsNullOrEmpty(filePath))
            {
                await uploadHandler.Remove(filePath);
            }

            await _context.SaveChangesAsync();

            return true;
        }
        public string? GetPhotoFileName(int userId, int id)
        {
            var contact = _context.Contacts
                                  .Where(c => c.UserId == userId && c.Id == id)
                                  .Include(c => c.Photo)
                                  .FirstOrDefault();

            return contact?.Photo?.FilePath;
        }
    }
}
