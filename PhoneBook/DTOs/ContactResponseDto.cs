using WebHomework.Models;

namespace WebHomework.DTOs
{
    public class ContactResponseDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public List<PhoneNumberResponseDto> PhoneNumbers { get; set; } = new List<PhoneNumberResponseDto>();
    }
}
