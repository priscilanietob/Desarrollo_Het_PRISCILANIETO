using System.Text.Json.Serialization;

namespace Comprehension.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }

        [JsonIgnore] // Nunca devolver el hash en una API
        public required byte[] PasswordHash { get; set; }

        [JsonIgnore] // Nunca devolver la sal en una API
        public required byte[] PasswordSalt { get; set; }

        // Relaciones
        [JsonIgnore]
        public virtual ICollection<Event> OwnedEvents { get; set; } = new List<Event>();
        [JsonIgnore]
        public virtual ICollection<Note> OwnedNotes { get; set; } = new List<Note>();
        [JsonIgnore]
        public virtual ICollection<Reminder> OwnedReminders { get; set; } = new List<Reminder>();
        [JsonIgnore]
        public virtual ICollection<AuthToken> AuthTokens { get; set; } = new List<AuthToken>();
        [JsonIgnore]
        public virtual ICollection<SharedPermission> SharedPermissions { get; set; } = new List<SharedPermission>();
    }
}