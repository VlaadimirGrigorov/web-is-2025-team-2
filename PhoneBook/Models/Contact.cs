using PhoneBook.Models;

namespace PhoneBook.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Photo? Photo { get; set; } // Потребителят може да има снимка
        public int UserId { get; set; } // Външен ключ към User
        public User? User { get; set; } // Навигационно свойство (da go mahnem?)
    }
}
