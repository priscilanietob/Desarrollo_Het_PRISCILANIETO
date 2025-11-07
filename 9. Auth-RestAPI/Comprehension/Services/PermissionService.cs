using Comprehension.Data;
using Comprehension.Models;
using Microsoft.EntityFrameworkCore;

namespace Comprehension.Services
{
    public class PermissionService
    {
        private readonly ComprehensionContext _context;

        public PermissionService(ComprehensionContext context)
        {
            _context = context;
        }

        // Verifica si el usuario es dueño O tiene permiso
        public async Task<bool> CheckPermission(Guid userId, Guid resourceId, string resourceType, PermissionLevel requiredLevel)
        {
            // 1. Verificar si el usuario es el dueño
            bool isOwner = false;
            switch (resourceType)
            {
                case "Event":
                    isOwner = await _context.Event.AnyAsync(e => e.Id == resourceId && e.OwnerId == userId);
                    break;
                case "Note":
                    isOwner = await _context.Note.AnyAsync(n => n.Id == resourceId && n.OwnerId == userId);
                    break;
                case "Reminder":
                    isOwner = await _context.Reminder.AnyAsync(r => r.Id == resourceId && r.OwnerId == userId);
                    break;
            }

            // El dueño siempre tiene todos los permisos
            if (isOwner)
            {
                return true;
            }

            // 2. Si no es el dueño, buscar en la tabla de permisos
            var permission = await _context.SharedPermissions
                .AsNoTracking()
                .FirstOrDefaultAsync(sp =>
                    sp.UserId == userId &&
                    sp.ResourceId == resourceId &&
                    sp.ResourceType == resourceType);

            if (permission == null)
            {
                return false; // No tiene permiso
            }

            // 3. Verificar si el nivel de permiso es suficiente
            // (Admin >= ReadWrite >= ReadOnly)
            return permission.PermissionLevel >= requiredLevel;
        }

        // Sobrecarga para permisos de "Admin" (Punto extra 3)
        public async Task<bool> IsAdminOrOwner(Guid userId, Guid resourceId, string resourceType)
        {
            return await CheckPermission(userId, resourceId, resourceType, PermissionLevel.Admin);
        }
    }
}