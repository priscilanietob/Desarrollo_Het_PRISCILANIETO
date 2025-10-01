namespace Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }   // ← este campo faltaba (clave primaria)

        public int ArticleId { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTimeOffset PublishedDate { get; set; }
    }
}
