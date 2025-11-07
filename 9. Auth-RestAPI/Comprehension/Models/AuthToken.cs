namespace Comprehension.Models
{
    public class AuthToken
    {
        public int Id { get; set; }
        public required string Token { get; set; } // El token de 256 bits
        public DateTime ExpiresAt { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}