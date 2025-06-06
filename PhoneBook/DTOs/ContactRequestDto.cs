using PhoneBook.Models;
using System.ComponentModel.DataAnnotations;

namespace PhoneBook.DTOs
{
    public class ContactRequestDto
    {
        [Required]
        public string? Name { get; set; }
        public string? Address { get; set; }
        [Required]
        public List<PhoneNumberRequestDto> PhoneNumbers { get; set; } = new List<PhoneNumberRequestDto>();
        public string? PhotoUrl { get; set; }
    }
}
