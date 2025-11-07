using Comprehension.Models;

namespace Comprehension.DTOs
{
    // --- EVENTOS ---
    public class CreateEventDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
        public List<ShareWithDto>? ShareWith { get; set; } // Opcional
    }
    public class UpdateEventDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
    }

    // --- NOTAS ---
    public class CreateNoteDto
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public List<ShareWithDto>? ShareWith { get; set; } // Opcional
    }
    public class UpdateNoteDto
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
    }

    // --- RECORDATORIOS ---
    public class CreateReminderDto
    {
        public required string Message { get; set; }
        public required DateTime ReminderTime { get; set; }
        public bool IsCompleted { get; set; } = false;
        public List<ShareWithDto>? ShareWith { get; set; } // Opcional
    }
    public class UpdateReminderDto
    {
        public required string Message { get; set; }
        public required DateTime ReminderTime { get; set; }
        public bool IsCompleted { get; set; }
    }
}