using System.ComponentModel.DataAnnotations;

namespace Auth.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        // Para almacenar la sal
        [Required]
        public byte[] PasswordSalt { get; set; }

        public string ProfileImageUrl { get; set; } 
        public virtual ICollection<UserSession> Sessions { get; set; }
    }
}