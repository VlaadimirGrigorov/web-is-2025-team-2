using System.ComponentModel.DataAnnotations;

namespace PhoneBook.DTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
