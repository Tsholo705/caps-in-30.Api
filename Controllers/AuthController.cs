using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        // POST: api/auth/register

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check whether the username is already taken
            if (await _context.Users.AnyAsync(
                u => u.Username.ToLower() == request.Username.ToLower()))
            {
                return BadRequest("Username is already taken.");
            }

            // Find the access code
            var accessCode = await _context.AccessCodes.FirstOrDefaultAsync(
                c => c.Code == request.AccessCode);

            // Check whether the code exists
            if (accessCode == null)
            {
                return BadRequest("Invalid access code.");
            }

            // Check whether the code has already been used
            if (accessCode.IsUsed)
            {
                return BadRequest("This access code has already been used.");
            }

            // Check whether the code matches the learner's grade
            if (accessCode.Grade != request.Grade)
            {
                return BadRequest("This access code is not valid for your grade.");
            }

            // Hash the password before saving it
            string hashedPassword = BC.HashPassword(request.Password);

            var newUser = new User
            {
                Name = request.Name,
                Username = request.Username,
                PasswordHash = hashedPassword,
                Grade = request.Grade
            };

            _context.Users.Add(newUser);

            // Mark the access code as used
            accessCode.IsUsed = true;
            accessCode.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful.",
                userId = newUser.Id,
                name = newUser.Name,
                username = newUser.Username,
                grade = newUser.Grade
            });
        }

        // POST: api/auth/login
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
            ClaimTypes.Name,
            user.Name
        ),

        new Claim(
            JwtRegisteredClaimNames.UniqueName,
            user.Username
        )
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
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