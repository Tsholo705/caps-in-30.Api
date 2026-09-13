using System.ComponentModel.DataAnnotations;

namespace caps_in_30.Api.Models
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int Grade { get; set; }

    }
}
