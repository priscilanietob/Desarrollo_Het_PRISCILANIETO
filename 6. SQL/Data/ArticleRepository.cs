using Blog.Models;
using Microsoft.Data.Sqlite;

namespace Blog.Data
{
    /// <summary>
    /// Implementation of <see cref="IArticleRepository"/> using SQLite as a persistence solution.
    /// </summary>
    public class ArticleRepository : IArticleRepository
    {
        private readonly string _connectionString;

        public ArticleRepository(DatabaseConfig _config)
        {
            _connectionString = _config.DefaultConnectionString ?? throw new ArgumentNullException("Connection string not found");
        }

        /// <summary>
        /// Creates the necessary tables for this application if they don't exist already.
        /// Should be called once when starting the service.
        /// </summary>
        public void EnsureCreated()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var createArticlesTable = @"
                CREATE TABLE IF NOT EXISTS Articles (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    AuthorName TEXT NOT NULL,
                    AuthorEmail TEXT NOT NULL,
                    Content TEXT NOT NULL,
                    PublishedDate TEXT NOT NULL
                );";

            var createCommentsTable = @"
                CREATE TABLE IF NOT EXISTS Comments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ArticleId INTEGER NOT NULL,
                    Content TEXT NOT NULL,
                    PublishedDate TEXT NOT NULL,
                    FOREIGN KEY (ArticleId) REFERENCES Articles(Id)
                );";

            using (var cmd = new SqliteCommand(createArticlesTable, connection))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SqliteCommand(createCommentsTable, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<Article> GetAll()
        {
            var articles = new List<Article>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = @"SELECT Id, Title, AuthorName, AuthorEmail, Content, PublishedDate 
                          FROM Articles 
                          ORDER BY PublishedDate DESC";

            using var cmd = new SqliteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                articles.Add(new Article
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    AuthorName = reader.GetString(2),
                    AuthorEmail = reader.GetString(3),
                    Content = reader.GetString(4),
                    PublishedDate = DateTime.Parse(reader.GetString(5))
                });
            }

            return articles;
        }

        public IEnumerable<Article> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var articles = new List<Article>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = @"SELECT Id, Title, AuthorName, AuthorEmail, Content, PublishedDate 
                          FROM Articles 
                          WHERE PublishedDate >= $start AND PublishedDate <= $end
                          ORDER BY PublishedDate DESC";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("$start", startDate.ToString("o"));
            cmd.Parameters.AddWithValue("$end", endDate.ToString("o"));

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                articles.Add(new Article
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    AuthorName = reader.GetString(2),
                    AuthorEmail = reader.GetString(3),
                    Content = reader.GetString(4),
                    PublishedDate = DateTime.Parse(reader.GetString(5))
                });
            }

            return articles;
        }

        public Article? GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = @"SELECT Id, Title, AuthorName, AuthorEmail, Content, PublishedDate 
                          FROM Articles 
                          WHERE Id = $id";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Article
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    AuthorName = reader.GetString(2),
                    AuthorEmail = reader.GetString(3),
                    Content = reader.GetString(4),
                    PublishedDate = DateTime.Parse(reader.GetString(5))
                };
            }

            return null;
        }

        public Article Create(Article article)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = @"
                INSERT INTO Articles (Title, AuthorName, AuthorEmail, Content, PublishedDate)
                VALUES ($title, $authorName, $authorEmail, $content, $publishedDate);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("$title", article.Title);
            cmd.Parameters.AddWithValue("$authorName", article.AuthorName);
            cmd.Parameters.AddWithValue("$authorEmail", article.AuthorEmail);
            cmd.Parameters.AddWithValue("$content", article.Content);
            cmd.Parameters.AddWithValue("$publishedDate", article.PublishedDate.ToString("o"));

            article.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return article;
        }

        public void AddComment(Comment comment)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = @"
                INSERT INTO Comments (ArticleId, Content, PublishedDate)
                VALUES ($articleId, $content, $publishedDate);";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("$articleId", comment.ArticleId);
            cmd.Parameters.AddWithValue("$content", comment.Content);
            cmd.Parameters.AddWithValue("$publishedDate", comment.PublishedDate.ToString("o"));

            cmd.ExecuteNonQuery();
        }

        public IEnumerable<Comment> GetCommentsByArticleId(int articleId)
        {
            var comments = new List<Comment>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = @"SELECT Id, ArticleId, Content, PublishedDate 
                          FROM Comments 
                          WHERE ArticleId = $articleId 
                          ORDER BY PublishedDate DESC";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("$articleId", articleId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                comments.Add(new Comment
                {
                    Id = reader.GetInt32(0),
                    ArticleId = reader.GetInt32(1),
                    Content = reader.GetString(2),
                    PublishedDate = DateTime.Parse(reader.GetString(3))
                });
            }

            return comments;
        }
    }
}