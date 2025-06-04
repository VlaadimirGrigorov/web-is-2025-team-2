using System.Text.Json.Serialization;

namespace PhoneBook.Models
{
    public class PhoneNumber
    {
        public int? Id { get; set; }
        public string? Number { get; set; }
        public int? ContactId { get; set; }  // foreign key
        [JsonIgnore]
        public Contact? Contact { get; set; } // navigation property
    }
}
