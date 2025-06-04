using PhoneBook.Models;

namespace PhoneBook.Models
{
    public class Photo
    {
        public int? Id { get; set; }
        public string FilePath { get; set; }
        public int? ContactId { get; set; }
        public Contact? Contact { get; set; }
    }
}
