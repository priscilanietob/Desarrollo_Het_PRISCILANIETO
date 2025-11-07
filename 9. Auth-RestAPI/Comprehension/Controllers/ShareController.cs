using Comprehension.Data;
using Comprehension.DTOs;
using Comprehension.Models;
using Comprehension.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Comprehension.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShareController : BaseApiController
    {
        private readonly ComprehensionContext _context;
        private readonly PermissionService _permissionService;

        public ShareController(ComprehensionContext context, PermissionService permissionService) : base(context)
        {
            _context = context;
            _permissionService = permissionService;
        }

        // POST api/share/grant
        [HttpPost("grant")]
        public async Task<IActionResult> GrantPermission(GrantPermissionDto dto)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized(); // 401

            // Verificar que el usuario actual es Admin o Dueño del recurso
            if (!await _permissionService.IsAdminOrOwner(CurrentUser.Id, dto.ResourceId, dto.ResourceType))
            {
                return Forbid("You do not have permission to share this resource."); // 403
            }

            var userToShare = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (userToShare == null) return NotFound("User to share with not found.");

            if (userToShare.Id == CurrentUser.Id) return BadRequest("Cannot share a resource with yourself.");

            var existingPermission = await _context.SharedPermissions.FirstOrDefaultAsync(sp =>
                sp.ResourceId == dto.ResourceId && sp.ResourceType == dto.ResourceType && sp.UserId == userToShare.Id);

            if (existingPermission != null)
            {
                existingPermission.PermissionLevel = dto.PermissionLevel;
            }
            else
            {
                _context.SharedPermissions.Add(new SharedPermission
                {
                    ResourceId = dto.ResourceId, ResourceType = dto.ResourceType,
                    UserId = userToShare.Id, PermissionLevel = dto.PermissionLevel
                });
            }

            await _context.SaveChangesAsync();
            return Ok("Permission granted/updated.");
        }

        // DELETE api/share/revoke
        [HttpDelete("revoke")]
        public async Task<IActionResult> RevokePermission(RevokePermissionDto dto)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized(); // 401

            // Verificar que el usuario actual es Admin o Dueño
            if (!await _permissionService.IsAdminOrOwner(CurrentUser.Id, dto.ResourceId, dto.ResourceType))
            {
                return Forbid("You do not have permission to modify this resource."); // 403
            }
            
            var userToRevoke = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (userToRevoke == null) return NotFound("User to revoke from not found.");

            var permission = await _context.SharedPermissions.FirstOrDefaultAsync(sp =>
                sp.ResourceId == dto.ResourceId && sp.ResourceType == dto.ResourceType && sp.UserId == userToRevoke.Id);

            if (permission == null) return NotFound("Permission not found.");

            _context.SharedPermissions.Remove(permission);
            await _context.SaveChangesAsync();

            return Ok("Permission revoked.");
        }
    }
}