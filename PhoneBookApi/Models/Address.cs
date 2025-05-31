namespace PhoneBookApi.Models
{
    public class Address
    {
        public int? Id { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public int? ContactId { get; set; }
        public Contact? Contact { get; set; }
    }
}