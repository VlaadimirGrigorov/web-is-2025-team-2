namespace WebHomework.Models
{
    public class Contact
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public List<PhoneNumber>? PhoneNumbers { get; set; } = new List<PhoneNumber>();
        public Address? Address { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
