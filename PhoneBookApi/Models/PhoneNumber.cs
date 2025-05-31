namespace PhoneBookApi.Models
{
    public class PhoneNumber
    {
        public int? Id { get; set; }
        public string? Number { get; set; } //число?
        public int? ContactId { get; set; } // Връзка към конкретния контакт
        public Contact? Contact { get; set; } 
    }
}