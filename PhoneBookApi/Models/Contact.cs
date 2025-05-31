namespace PhoneBookApi.Models
{
    public class Contact
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; } //по-добре само като стринг тук, но може да се добави класа във front-end-a
        public string? PhotoPath { get; set; } // Път към снимката
        public List<PhoneNumber> PhoneNumbers { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}