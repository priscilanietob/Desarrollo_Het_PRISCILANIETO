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
    public class RemindersController : BaseApiController
    {
        private readonly ComprehensionContext _context;
        private readonly PermissionService _permissionService;

        public RemindersController(ComprehensionContext context, PermissionService permissionService) : base(context)
        {
            _context = context;
            _permissionService = permissionService;
        }

        // GET: api/Reminders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reminder>>> GetReminder()
        {
            if (!await AuthenticateUserAsync()) return Unauthorized();

            var sharedIds = await _context.SharedPermissions
                .Where(sp => sp.UserId == CurrentUser!.Id && sp.ResourceType == "Reminder") // <- FIX
                .Select(sp => sp.ResourceId)
                .ToListAsync();

            return await _context.Reminder
                .Where(r => r.OwnerId == CurrentUser!.Id || sharedIds.Contains(r.Id)) // <- FIX
                .ToListAsync();
        }

        // GET: api/Reminders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reminder>> GetReminder(Guid id)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized();

            if (!await _permissionService.CheckPermission(CurrentUser!.Id, id, "Reminder", PermissionLevel.ReadOnly)) // <- FIX
            {
                return Forbid(); // 403
            }

            var reminder = await _context.Reminder.FindAsync(id);
            if (reminder == null) return NotFound();
            return reminder;
        }

        // PUT: api/Reminders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReminder(Guid id, UpdateReminderDto dto)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized();

            if (!await _permissionService.CheckPermission(CurrentUser!.Id, id, "Reminder", PermissionLevel.ReadWrite)) // <- FIX
            {
                return Forbid(); // 403
            }

            var reminder = await _context.Reminder.FindAsync(id);
            if (reminder == null) return NotFound();

            reminder.Message = dto.Message;
            reminder.ReminderTime = dto.ReminderTime;
            reminder.IsCompleted = dto.IsCompleted;
            _context.Entry(reminder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reminder.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // POST: api/Reminders
        [HttpPost]
        public async Task<ActionResult<Reminder>> PostReminder(CreateReminderDto dto)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized();

            var reminder = new Reminder
            {
                Message = dto.Message,
                ReminderTime = dto.ReminderTime,
                IsCompleted = dto.IsCompleted,
                OwnerId = CurrentUser!.Id // <- FIX
            };
            _context.Reminder.Add(reminder);

            if (dto.ShareWith != null && dto.ShareWith.Any())
            {
                await HandleSharingOnCreate(reminder.Id, "Reminder", dto.ShareWith);
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetReminder", new { id = reminder.Id }, reminder);
        }

        // DELETE: api/Reminders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(Guid id)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized();

            if (!await _permissionService.CheckPermission(CurrentUser!.Id, id, "Reminder", PermissionLevel.Admin)) // <- FIX
            {
                return Forbid(); // 403
            }

            var reminder = await _context.Reminder.FindAsync(id);
            if (reminder == null) return NotFound();

            _context.Reminder.Remove(reminder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper
        private async Task HandleSharingOnCreate(Guid resourceId, string resourceType, List<ShareWithDto> shareList)
        {
            foreach (var share in shareList)
            {
                var userToShare = await _context.Users.FirstOrDefaultAsync(u => u.Username == share.Username);
                if (userToShare != null && userToShare.Id != CurrentUser!.Id) // <- FIX
                {
                    _context.SharedPermissions.Add(new SharedPermission
                    {
                        ResourceId = resourceId, ResourceType = resourceType,
                        UserId = userToShare.Id, PermissionLevel = share.PermissionLevel
                    });
                }
            }
        }
    }
}