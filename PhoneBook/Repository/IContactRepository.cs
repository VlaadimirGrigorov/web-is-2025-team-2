using Microsoft.AspNetCore.Mvc;
using PhoneBook.DTOs;
using PhoneBook.Models;

namespace PhoneBook.Repository
{
    public interface IContactRepository
    {
        Task<List<ContactResponseDto>> GetContacts(int userId);
        Task<ContactResponseDto?> GetContact(int userId, int id);
        Task<RepositoryResult<ContactResponseDto>> AddContact(int userId, ContactRequestDto contactDto);
        Task<ContactResponseDto> DeleteContact(int userId, int id);
        Task<ContactResponseDto> UpdateContact(int userId, int id, ContactRequestDto updatedContactDto);
        Task<RepositoryResult<PhoneNumberResponseDto>> AddPhoneToContact(int userId, int contactId, PhoneNumberRequestDto dto);
        Task<RepositoryResult<PhoneNumberResponseDto>> UpdatePhoneNumberInContact(int userId, int id, int phoneId, PhoneNumberRequestDto phoneNumberDto);
        Task<RepositoryResult<PhoneNumberResponseDto>> DeletePhoneNumberFromContact(int userId, int id, int phoneId);
        Task<ContactResponseDto> AddPhotoToContact(int userId, int contactId, string filePath);
        Task<bool> DeletePhotoFromContact(int userId, int contactId);
        string? GetPhotoFileName(int userId, int id);
        Task<List<ContactResponseDto>> SearchContactsAsync(int userId, string searchTerm, int limit);

    }
}
