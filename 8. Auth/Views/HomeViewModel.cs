using Auth.Models;

namespace Auth.Models
{
    public class HomeViewModel
    {
        public RegisterViewModel RegisterModel { get; set; }
        public LoginViewModel LoginModel { get; set; }
    }
}