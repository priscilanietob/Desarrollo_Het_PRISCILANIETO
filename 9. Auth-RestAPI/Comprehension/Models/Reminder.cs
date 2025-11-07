using System.Text.Json.Serialization;

namespace Comprehension.Models
{
    public class Reminder
    {
        public Guid Id { get; internal set; }

        public required string Message { get; set; }

        public required DateTime ReminderTime { get; set; }

        public bool IsCompleted { get; set; } = false;

        // --- AÑADIR ESTO ---
        public Guid OwnerId { get; set; }
        [JsonIgnore]
        public virtual User Owner { get; set; }
        // --------------------
    }
}