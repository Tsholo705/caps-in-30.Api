using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using caps_in_30.Api.Data;
using caps_in_30.Api.Models;
using BC = BCrypt.Net.BCrypt;

namespace caps_in_30.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(
            ApplicationDbContext context,
            IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Users.AnyAsync(
                u => u.Username.ToLower() == request.Username.ToLower()))
            {
                return BadRequest("Username is already taken.");
            }

            string hashedPassword = BC.HashPassword(request.Password);

            var newUser = new User
            {
                Name = request.Name,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Grade = request.Grade,
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful!",
                userId = newUser.Id,
                name = newUser.Name,
                username = newUser.Username,
                email = newUser.Email,
                grade = newUser.Grade
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(
                u => u.Username.ToLower() == request.Username.ToLower());

            if (user == null)
            {
                return BadRequest("Invalid username or password.");
            }

            if (!BC.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Invalid username or password.");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                message = "Login successful!",
                token = token,
                userId = user.Id,
                name = user.Name,
                username = user.Username,
                email = user.Email,
                grade = user.Grade
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email
                ),

                new Claim(
                    ClaimTypes.Name,
                    user.Name
                )
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _config["Jwt:Key"]!
                )
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var duration = double.Parse(
                _config["Jwt:DurationInMinutes"] ?? "60"
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),

                Expires = DateTime.UtcNow.AddMinutes(duration),

                Issuer = _config["Jwt:Issuer"],

                Audience = _config["Jwt:Audience"],

                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}