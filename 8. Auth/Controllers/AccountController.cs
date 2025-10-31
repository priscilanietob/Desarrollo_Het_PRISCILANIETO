using Auth.Data;
using Auth.Models;
using Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System.Security.Cryptography;

namespace Auth.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email))
            {
                ModelState.AddModelError(string.Empty, "El nombre de usuario o email ya están en uso.");
                return View(model);
            }

            var (hash, salt) = PasswordHasher.HashPassword(model.Password);            
            var userCount = await _context.Users.CountAsync();
            string assignedImageUrl;

            if (userCount == 0)
            {
                assignedImageUrl = "~/images/snoopy.jpeg";
            }
            else
            {
                assignedImageUrl = "~/images/snoopy1.jpeg";
            }


            var user = new User
            {
                Username = model.Username,
                Name = model.Name,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                PasswordHash = hash,
                PasswordSalt = salt,
                ProfileImageUrl = assignedImageUrl 
            };
            

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "¡Te has registrado exitosamente! Ahora puedes iniciar sesión.";

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null || !PasswordHasher.VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos.");
                return View(model);
            }

            var sessionIdBytes = RandomNumberGenerator.GetBytes(16);
            var sessionId = Convert.ToBase64String(sessionIdBytes);

            var session = new UserSession
            {
                SessionId = sessionId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow
            };

            await _context.UserSessions.AddAsync(session);
            await _context.SaveChangesAsync();

            Response.Cookies.Append("AuthSessionId", sessionId, new CookieOptions
            {
                HttpOnly = true, 
                Secure = true,   
                SameSite = SameSiteMode.Strict 
            });

            TempData["SuccessMessage"] = $"¡Bienvenido de nuevo, {user.Name}!";

            return RedirectToAction("Privacy", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("AuthSessionId", out var sessionId))
            {
                var session = await _context.UserSessions.FirstOrDefaultAsync(s => s.SessionId == sessionId);
                if (session != null)
                {
                    _context.UserSessions.Remove(session);
                    await _context.SaveChangesAsync();
                }
                Response.Cookies.Delete("AuthSessionId");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}