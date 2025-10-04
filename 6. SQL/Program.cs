using Blog.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder);
            var app = builder.Build();
            ConfigureMiddleware(app);

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllersWithViews();

            // Configuración de base de datos con SQLite
            builder.Services.AddDbContext<BlogDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configuración de repositorio en memoria o EF
            var databaseConfig = builder.Configuration
                                        .GetSection("DatabaseConfig")
                                        .Get<DatabaseConfig>();

            if (databaseConfig != null && databaseConfig.UseInMemoryDatabase)
            {
                builder.Services.AddSingleton<IArticleRepository, MemoryArticleRepository>();
            }
            else
            {
                builder.Services.AddScoped<IArticleRepository, EfArticleRepository>();
            }
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Articles}/{action=Index}/{id?}");
        }
    }
}