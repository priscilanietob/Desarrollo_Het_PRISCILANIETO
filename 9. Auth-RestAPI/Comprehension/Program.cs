using Comprehension.Data;
using Microsoft.EntityFrameworkCore;
using Comprehension.Services; // Importar servicios

namespace Comprehension
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ComprehensionContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("ComprehensionContext") ?? throw new InvalidOperationException("Connection string 'ComprehensionContext' not found.")));

            // Add services to the container.

            // --- AÑADIR ESTO: Registrar servicios ---
            builder.Services.AddScoped<PasswordService>();
            builder.Services.AddSingleton<TokenService>(); // Singleton está bien para este
            builder.Services.AddScoped<PermissionService>();
            // ----------------------------------------

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // No se añade ningún middleware personalizado aquí,
            // cumpliendo con tu restricción.
            
            app.UseAuthorization(); // Mantener esto

            app.MapControllers();

            app.Run();
        }
    }
}