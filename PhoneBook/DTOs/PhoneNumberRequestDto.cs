using PhoneBook.Validators; // така зареждаш PhoneNumberAttribute

namespace PhoneBook.DTOs
{
    public class PhoneNumberRequestDto
    {
        [PhoneNumber]
        public string Number { get; set; }
    }
}