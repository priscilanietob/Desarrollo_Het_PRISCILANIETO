namespace Blog.Models
{
    public class Comment
    {
        public int Id { get; set; } 
        public int ArticleId { get; set; } 
        public string Content { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; } = DateTime.UtcNow; 
        public Article? Article { get; set; } = null;
    }
}