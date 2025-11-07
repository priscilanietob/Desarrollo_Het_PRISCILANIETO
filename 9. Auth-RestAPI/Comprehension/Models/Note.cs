using System.Text.Json.Serialization;

namespace Comprehension.Models
{
    public class Note
    {
        public Guid Id { get; internal set; }

        public required string Title { get; set; }

        public required string Content { get; set; }

        public DateTime CreatedAt { get; internal set; }

        public DateTime UpdatedAt { get; internal set; }

        // --- AÑADIR ESTO ---
        public Guid OwnerId { get; set; }
        [JsonIgnore]
        public virtual User Owner { get; set; }
        // --------------------
    }
}