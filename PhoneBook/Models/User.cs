using PhoneBook.Models;

namespace PhoneBook.Models
{
    public class User
    {
        public int? Id { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; } // За сигурност, съхранявай хеш на паролата
        public string? Email { get; set; }
        public List<Contact>? Contacts { get; set; } // Потребителят има своите контакти
    }
}
