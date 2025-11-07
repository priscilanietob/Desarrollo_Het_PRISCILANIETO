namespace Comprehension.Models
{
    public class SharedPermission
    {
        public int Id { get; set; }
        public Guid ResourceId { get; set; } // ID del Event, Note, o Reminder
        public required string ResourceType { get; set; } // "Event", "Note", "Reminder"

        public Guid UserId { get; set; } // Usuario con quien se comparte
        public virtual User User { get; set; }

        public PermissionLevel PermissionLevel { get; set; }
    }
}