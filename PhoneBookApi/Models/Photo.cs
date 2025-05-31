namespace PhoneBookApi.Models
{
    public class Photo
    {
        public int? Id { get; set; }
        public string? FilePath { get; set; } // Път към снимката
        public int? ContactId { get; set; }
        public Contact? Contact { get; set; }
    }
}