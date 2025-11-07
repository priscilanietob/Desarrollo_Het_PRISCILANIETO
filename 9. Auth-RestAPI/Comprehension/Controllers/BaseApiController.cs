using Comprehension.Data;
using Comprehension.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Comprehension.Controllers
{
    // No tiene [ApiController] ni [Route] a propósito.
    // Es solo una clase base.
    public abstract class BaseApiController : ControllerBase
    {
        private readonly ComprehensionContext _dbContext;
        protected User? CurrentUser { get; private set; }

        public BaseApiController(ComprehensionContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Esta función reemplaza al [AuthorizeAttribute] y al AuthMiddleware.
        /// Debe ser llamada al inicio de CADA método de controlador que requiera autenticación.
        /// </summary>
        /// <returns>
        /// Retorna 'true' si el usuario está autenticado y 'CurrentUser' se ha establecido.
        /// Retorna 'false' si el token es inválido, no existe o ha expirado.
        /// </returns>
        protected async Task<bool> AuthenticateUserAsync()
        {
            // Si ya lo autenticamos en esta petición, no buscar de nuevo.
            if (this.CurrentUser != null) return true;

            // 1. Leer la cabecera
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return false; // No hay token
            }

            // 2. Obtener el token
            string token = authHeader.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token)) return false;

            // 3. Buscar en la BD
            var authToken = await _dbContext.AuthTokens
                .Include(t => t.User)
                .AsNoTracking() // Es una lectura, no necesitamos rastrearlo
                .FirstOrDefaultAsync(t => t.Token == token);

            // 4. Validar que exista y no esté expirado
            if (authToken == null || authToken.ExpiresAt <= DateTime.UtcNow)
            {
                return false; // Token no encontrado o expirado
            }

            // 5. ¡Éxito! Establecer el usuario para esta petición.
            this.CurrentUser = authToken.User;
            return true;
        }
    }
}