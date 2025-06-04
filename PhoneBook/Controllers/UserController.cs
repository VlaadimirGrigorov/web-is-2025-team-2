using Microsoft.AspNetCore.Mvc;
using PhoneBook.Models;
using PhoneBook.Data;
using PhoneBook.Helpers;
using Microsoft.EntityFrameworkCore;
using PhoneBook.DTOs;

namespace PhoneBook.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PhoneBookContext _context;

        public UsersController(PhoneBookContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            var email = await _context.Users.FirstOrDefaultAsync(c => c.Email == dto.Email);
            if (email != null)
            {
                return Conflict("Email already exists!");
            }

            var username = await _context.Users.FirstOrDefaultAsync(c => c.Username == dto.Username);
            if (username != null)
            {
                return Conflict("Username already exists!");
            }

            // Хешираме паролата (например с SHA256 или друга библиотека bcrypt)
            string passwordHash = GenericHelpers.ComputeSha256Hash(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Можем да върнем само базова информация за новия user (без парола)
            return CreatedAtAction(nameof(Register), new { id = user.Id }, new { user.Id, user.Username, user.Email });
        }
    }
}
