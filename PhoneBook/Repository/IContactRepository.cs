using Microsoft.AspNetCore.Mvc;
using PhoneBook.DTOs;
using PhoneBook.Models;

namespace PhoneBook.Repository
{
    public interface IContactRepository
    {
        Task<List<ContactResponseDto>> GetContacts();
        Task<ContactResponseDto?> GetContact(int id);
        Task<ContactResponseDto> AddContact(ContactRequestDto contactDto);
        Task<ContactResponseDto> DeleteContact(int id);
        Task<ContactResponseDto> UpdateContact(int id, ContactRequestDto updatedContactDto);
        Task<RepositoryResult<PhoneNumberResponseDto>> AddPhoneToContact(int contactId, PhoneNumberRequestDto dto);
        Task<RepositoryResult<PhoneNumberResponseDto>> UpdatePhoneNumberInContact(int id, int phoneId, PhoneNumberRequestDto phoneNumberDto);
        Task<RepositoryResult<PhoneNumberResponseDto>> DeletePhoneNumberFromContact(int id, int phoneId);
    }
}
