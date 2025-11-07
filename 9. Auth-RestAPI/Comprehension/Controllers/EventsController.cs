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
    public class EventsController : BaseApiController
    {
        private readonly ComprehensionContext _context;
        private readonly PermissionService _permissionService;

        public EventsController(ComprehensionContext context, PermissionService permissionService) : base(context)
        {
            _context = context;
            _permissionService = permissionService;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvent()
        {
            if (!await AuthenticateUserAsync()) return Unauthorized(); // 401

            // Buscar IDs compartidos
            var sharedIds = await _context.SharedPermissions
                .Where(sp => sp.UserId == CurrentUser!.Id && sp.ResourceType == "Event") // <- FIX
                .Select(sp => sp.ResourceId)
                .ToListAsync();

            // Devolver propios Y compartidos
            var events = await _context.Event
                .Where(e => e.OwnerId == CurrentUser!.Id || sharedIds.Contains(e.Id)) // <- FIX
                .ToListAsync();

            return events;
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(Guid id)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized(); // 401

            if (!await _permissionService.CheckPermission(CurrentUser!.Id, id, "Event", PermissionLevel.ReadOnly)) // <- FIX
            {
                return Forbid(); // 403
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null) return NotFound();
            return @event;
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(Guid id, UpdateEventDto dto)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized(); // 401
            
            if (!await _permissionService.CheckPermission(CurrentUser!.Id, id, "Event", PermissionLevel.ReadWrite)) // <- FIX
            {
                return Forbid(); // 403
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null) return NotFound();

            if (dto.StartTime >= dto.EndTime) return BadRequest("Invalid data.");

            // Actualizar propiedades
            @event.Title = dto.Title;
            @event.Description = dto.Description;
            @event.StartTime = dto.StartTime;
            @event.EndTime = dto.EndTime;
            _context.Entry(@event).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Event.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // POST: api/Events
        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent(CreateEventDto dto)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized(); // 401

            if (dto.StartTime >= dto.EndTime) return BadRequest("Invalid e data.");

            var @event = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                OwnerId = CurrentUser!.Id // <- FIX
            };
            _context.Event.Add(@event);
            
            if (dto.ShareWith != null && dto.ShareWith.Any())
            {
                await HandleSharingOnCreate(@event.Id, "Event", dto.ShareWith);
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetEvent", new { id = @event.Id }, @event);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized(); // 401

            if (!await _permissionService.CheckPermission(CurrentUser!.Id, id, "Event", PermissionLevel.Admin)) // <- FIX
            {
                return Forbid(); // 403
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null) return NotFound();

            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper para compartir en POST
        private async Task HandleSharingOnCreate(Guid resourceId, string resourceType, List<ShareWithDto> shareList)
        {
            foreach (var share in shareList)
            {
                var userToShare = await _context.Users.FirstOrDefaultAsync(u => u.Username == share.Username);
                if (userToShare != null && userToShare.Id != CurrentUser!.Id) // <- FIX
                {
                    _context.SharedPermissions.Add(new SharedPermission
                    {
                        ResourceId = resourceId,
                        ResourceType = resourceType,
                        UserId = userToShare.Id,
                        PermissionLevel = share.PermissionLevel
                    });
                }
            }
        }
    }
}