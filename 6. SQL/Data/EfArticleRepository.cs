using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class EfArticleRepository : IArticleRepository
    {
        private readonly BlogDbContext _context;

        public EfArticleRepository(BlogDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Article> GetAll()
        {
            return _context.Articles
                .Include(a => a.Comments)
                .OrderByDescending(a => a.PublishedDate)
                .ToList();
        }

        public IEnumerable<Article> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.Articles
                .Include(a => a.Comments)
                .Where(a => a.PublishedDate >= startDate && a.PublishedDate <= endDate)
                .OrderByDescending(a => a.PublishedDate)
                .ToList();
        }

        public Article? GetById(int id)
        {
            return _context.Articles
                .Include(a => a.Comments)
                .FirstOrDefault(a => a.Id == id);
        }

        public Article Create(Article article)
        {
            _context.Articles.Add(article);
            _context.SaveChanges();
            return article;
        }

        public IEnumerable<Comment> GetCommentsByArticleId(int articleId)
        {
            return _context.Comments
                .Where(c => c.ArticleId == articleId)
                .OrderByDescending(c => c.PublishedDate)
                .ToList();
        }

        public void AddComment(Comment comment)
        {
            if (!_context.Articles.Any(a => a.Id == comment.ArticleId))
            {
                throw new ArgumentException("Invalid ArticleId for comment.");
            }

            _context.Comments.Add(comment);
            _context.SaveChanges();
        }
    }
}
