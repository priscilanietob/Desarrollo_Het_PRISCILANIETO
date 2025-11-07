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
    public class NotesController : BaseApiController
    {
        private readonly ComprehensionContext _context;
        private readonly PermissionService _permissionService;

        public NotesController(ComprehensionContext context, PermissionService permissionService) : base(context)
        {
            _context = context;
            _permissionService = permissionService;
        }

        // GET: api/Notes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetNote()
        {
            if (!await AuthenticateUserAsync()) return Unauthorized();

            var sharedIds = await _context.SharedPermissions
                .Where(sp => sp.UserId == CurrentUser!.Id && sp.ResourceType == "Note") // <- FIX
                .Select(sp => sp.ResourceId)
                .ToListAsync();

            return await _context.Note
                .Where(n => n.OwnerId == CurrentUser!.Id || sharedIds.Contains(n.Id)) // <- FIX
                .ToListAsync();
        }

        // GET: api/Notes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(Guid id)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized();

            if (!await _permissionService.CheckPermission(CurrentUser!.Id, id, "Note", PermissionLevel.ReadOnly)) // <- FIX
            {
                return Forbid(); // 403
            }

            var note = await _context.Note.FindAsync(id);
            if (note == null) return NotFound();
            return note;
        }

        // PUT: api/Notes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(Guid id, UpdateNoteDto dto)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized();

            if (!await _permissionService.CheckPermission(CurrentUser!.Id, id, "Note", PermissionLevel.ReadWrite)) // <- FIX
            {
                return Forbid(); // 403
            }
            
            var note = await _context.Note.FindAsync(id);
            if (note == null) return NotFound();

            note.Title = dto.Title;
            note.Content = dto.Content;
            note.UpdatedAt = DateTime.UtcNow;
            _context.Entry(note).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Note.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // POST: api/Notes
        [HttpPost]
        public async Task<ActionResult<Note>> PostNote(CreateNoteDto dto)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized();

            var note = new Note
            {
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OwnerId = CurrentUser!.Id // <- FIX
            };
            _context.Note.Add(note);

            if (dto.ShareWith != null && dto.ShareWith.Any())
            {
                await HandleSharingOnCreate(note.Id, "Note", dto.ShareWith);
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetNote", new { id = note.Id }, note);
        }

        // DELETE: api/Notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            if (!await AuthenticateUserAsync()) return Unauthorized();

            if (!await _permissionService.CheckPermission(CurrentUser!.Id, id, "Note", PermissionLevel.Admin)) // <- FIX
            {
                return Forbid(); // 403
            }

            var note = await _context.Note.FindAsync(id);
            if (note == null) return NotFound();

            _context.Note.Remove(note);
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