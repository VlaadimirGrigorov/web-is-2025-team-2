using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PhoneBook.Data;
using PhoneBook.DTOs;
using PhoneBook.Helpers;
using PhoneBook.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PhoneBook.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PhoneBookContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthController(PhoneBookContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            // Търсим user по username (или email) – тук показвам пример само с username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username.");
            }

            // Проверяваме дали подадената парола съвпада с хеша в базата
            var incomingHash = GenericHelpers.ComputeSha256Hash(dto.Password);
            if (incomingHash != user.PasswordHash)
            {
                return Unauthorized("Invalid password.");
            }

            // Ако всичко е ОК – генерираме JWT
            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenDurationMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
