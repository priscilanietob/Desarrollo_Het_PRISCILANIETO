using Comprehension.Data;
using Comprehension.Models;
using Comprehension.Services;
using Comprehension.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Comprehension.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController // Hereda para usar AuthenticateUserAsync en Logout
    {
        private readonly ComprehensionContext _context;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;
        private const int TokenValidityMinutes = 60;

        public AuthController(ComprehensionContext context, PasswordService passwordService, TokenService tokenService) : base(context)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return BadRequest("Username already exists.");
            }

            var (hash, salt) = _passwordService.HashPassword(dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !_passwordService.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized("Invalid username or password."); // 401
            }

            var tokenString = _tokenService.GenerateToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(TokenValidityMinutes);

            var authToken = new AuthToken
            {
                Token = tokenString,
                ExpiresAt = expiresAt,
                UserId = user.Id
            };

            _context.AuthTokens.Add(authToken);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                Token = tokenString,
                ExpiresAt = expiresAt,
                Username = user.Username
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // El logout SÍ requiere autenticación
            if (!await AuthenticateUserAsync())
            {
                return Unauthorized(); // 401
            }
            
            // Obtener el token de la cabecera
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var authToken = await _context.AuthTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (authToken != null)
            {
                _context.AuthTokens.Remove(authToken);
                await _context.SaveChangesAsync();
            }

            return Ok(new { Message = "Logged out successfully." });
        }
    }
}