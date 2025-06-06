using PhoneBook.Models;

namespace PhoneBook.DTOs
{
    public class ContactResponseDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public List<PhoneNumberResponseDto> PhoneNumbers { get; set; } = new List<PhoneNumberResponseDto>();
        public string? PhotoUrl { get; set; }
    }
}
