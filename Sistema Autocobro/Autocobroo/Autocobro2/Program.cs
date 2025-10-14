using Microsoft.EntityFrameworkCore;
using Autocobro.Models;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration; // Incluido para la configuración flexible

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite("Data Source=autocobro.db"));

var app = builder.Build();

// Cargar datos iniciales desde el CSV si la base está vacía
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    context.Database.EnsureCreated();

    if (!context.Productos.Any())
    {
        using var reader = new StreamReader("catalogo_productos_mx.csv");
        
        // Configuración para hacer la lectura de encabezados flexible (solución para MissingFieldException)
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
            TrimOptions = TrimOptions.Trim | TrimOptions.None,
        };
        
        using var csv = new CsvReader(reader, config);
        
        // Aquí es donde ocurría el error de TypeConverterException por el formato del precio
        var productos = csv.GetRecords<Producto>().ToList(); 
        
        context.Productos.AddRange(productos);
        context.SaveChanges();
    }
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();