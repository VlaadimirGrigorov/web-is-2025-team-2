using WebHomework.Models;

namespace WebHomework.DTOs
{
    public class ContactRequestDto
    {
        public string? Name { get; set; }
        public AddressDto? Address { get; set; }
        public List<PhoneNumberRequestDto> PhoneNumbers { get; set; } = new List<PhoneNumberRequestDto>();
    }
}
