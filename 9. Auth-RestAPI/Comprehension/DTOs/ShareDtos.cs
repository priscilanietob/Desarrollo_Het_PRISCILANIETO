using Comprehension.Models;
using System.ComponentModel.DataAnnotations;

namespace Comprehension.DTOs
{
    // DTO para compartir (reutilizable)
    public class ShareWithDto
    {
        public required string Username { get; set; }
        public PermissionLevel PermissionLevel { get; set; } = PermissionLevel.Admin;
    }

    // DTOs para Grant/Revoke (Puntos extra)
    public class GrantPermissionDto
    {
        [Required]
        public Guid ResourceId { get; set; }
        [Required]
        public required string ResourceType { get; set; } // "Event", "Note", "Reminder"
        [Required]
        public required string Username { get; set; } // Usuario a quien compartir
        [Required]
        public PermissionLevel PermissionLevel { get; set; }
    }

    public class RevokePermissionDto
    {
        [Required]
        public Guid ResourceId { get; set; }
        [Required]
        public required string ResourceType { get; set; }
        [Required]
        public required string Username { get; set; } // Usuario a quien revocar
    }
}