using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly IArticleRepository _articleRepository;

        public ArticlesController(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public ActionResult Index(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? authorEmail = null,
            [FromQuery] string? title = null)
        {
            var articles = _articleRepository.GetAll().AsQueryable();

            // filtros aplicados 
            if (startDate is not null)
                articles = articles.Where(a => a.PublishedDate >= startDate.Value);

            if (endDate is not null)
                articles = articles.Where(a => a.PublishedDate <= endDate.Value);

            if (!string.IsNullOrEmpty(authorEmail))
                articles = articles.Where(a => a.AuthorEmail.ToLower() == authorEmail.ToLower());

            if (!string.IsNullOrEmpty(title))
                articles = articles.Where(a => a.Title.ToLower().Contains(title.ToLower()));

            return View(articles.ToList());
        }

        public ActionResult Details(int id)
        {
            var article = _articleRepository.GetById(id);
            if (article is null) return NotFound();

            var comments = _articleRepository.GetCommentsByArticleId(id);
            var viewModel = new ArticleDetailsViewModel(article, comments);

            return View(viewModel);
        }

        public ActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Article article)
        {
            if (!ModelState.IsValid)
                return View(article);

            article.PublishedDate = DateTime.UtcNow;
            var created = _articleRepository.Create(article);

            return RedirectToAction(nameof(Details), new { id = created.Id });
        }

        [HttpPost]
        [Route("Articles/{articleId}/AddComment")]
        public ActionResult AddComment(int articleId, Comment comment)
        {
            var article = _articleRepository.GetById(articleId);
            if (article is null) return NotFound();

            if (string.IsNullOrWhiteSpace(comment.Content))
                return BadRequest();

            comment.ArticleId = articleId;
            comment.PublishedDate = DateTime.UtcNow;

            _articleRepository.AddComment(comment);

            return RedirectToAction(nameof(Details), new { id = articleId });
        }
    }
}