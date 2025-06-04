using PhoneBook.DTOs;
using PhoneBook.Models;

namespace PhoneBook.Mappers
{
    public static class DtoMappers
    {
        public static PhoneNumberResponseDto ToDto(PhoneNumber phoneNumber)
        {
            return new PhoneNumberResponseDto
            {
                Id = phoneNumber.Id,
                Number = phoneNumber.Number
            };
        }

        public static PhoneNumber ToEntity(PhoneNumberRequestDto dto)
        {
            return new PhoneNumber
            {
                Number = dto.Number
            };
        }

        public static ContactResponseDto ToDto(Contact contact)
        {
            return new ContactResponseDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Address = contact.Address,
                PhoneNumbers = contact.PhoneNumbers
                    .Select(p => new PhoneNumberResponseDto
                    {
                        Id = p.Id,
                        Number = p.Number
                    }).ToList()
            };
        }

        public static Contact ToEntity(ContactRequestDto dto)
        {
            return new Contact
            {
                Name = dto.Name,
                Address = dto.Address,
                PhoneNumbers = dto.PhoneNumbers
                    .Select(p => new PhoneNumber
                    {
                        Number = p.Number
                    }).ToList()
            };
        }
    }
}
