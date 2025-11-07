using System.ComponentModel.DataAnnotations;

namespace Comprehension.DTOs
{
    public class RegisterDto
    {
        [Required][MinLength(3)]
        public required string Username { get; set; }
        [Required][MinLength(8)]
        public required string Password { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public required string Username { get; set; }
    }
}