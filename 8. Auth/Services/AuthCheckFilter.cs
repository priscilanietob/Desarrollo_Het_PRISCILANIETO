using Auth.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Auth.Services
{
    public class AuthCheckFilter : IAsyncActionFilter
    {
        private readonly AppDbContext _context;
        public AuthCheckFilter(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Cookies.TryGetValue("AuthSessionId", out var sessionId))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var session = await _context.UserSessions
                                .Include(s => s.User) 
                                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var timeSinceLastAccess = DateTime.UtcNow - session.LastAccessedAt;
            if (timeSinceLastAccess.TotalMinutes > 5)
            {
                _context.UserSessions.Remove(session); 
                await _context.SaveChangesAsync();
                context.HttpContext.Response.Cookies.Delete("AuthSessionId"); 
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            session.LastAccessedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            context.HttpContext.Items["User"] = session.User;

            await next();
        }
    }
}