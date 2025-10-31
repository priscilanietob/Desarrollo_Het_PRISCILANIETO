using System.ComponentModel.DataAnnotations;

namespace Auth.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}