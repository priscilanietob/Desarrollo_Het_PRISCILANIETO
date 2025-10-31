using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Models
{
    public class UserSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public string SessionId { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastAccessedAt { get; set; }
    }
}