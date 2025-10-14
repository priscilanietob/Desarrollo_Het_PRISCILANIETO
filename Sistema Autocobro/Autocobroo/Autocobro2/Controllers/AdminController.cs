using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Autocobro.Models;

namespace Autocobro.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataContext _context;

        public AdminController(DataContext context)
        {
            _context = context;
        }

        // Ruta: /Admin
        public IActionResult Index()
        {
            return View();
        }

        // Ruta: /Admin/Tickets
        public IActionResult Tickets()
        {
            return View();
        }

        // Ruta: /Admin/Productos
        public IActionResult Productos()
        {
            return View();
        }

        // Ruta: /Admin/Reportes
        public IActionResult Reportes()
        {
            var hoy = DateTime.Today;
            var ticketsHoy = _context.Tickets
                .Include(t => t.Items)
                .ThenInclude(i => i.Producto)
                .Where(t => t.Fecha.Date == hoy)
                .ToList();

            var reporte = new Reporte
            {
                VentasTotales = ticketsHoy.Count,
                IngresosTotales = ticketsHoy.Sum(t => t.Items.Sum(i => i.Producto.Precio * i.Cantidad)),
                IngresosEfectivo = ticketsHoy
                    .Where(t => t.MetodoPago == "Efectivo")
                    .Sum(t => t.Items.Sum(i => i.Producto.Precio * i.Cantidad)),
                IngresosTarjeta = ticketsHoy
                    .Where(t => t.MetodoPago == "Tarjeta")
                    .Sum(t => t.Items.Sum(i => i.Producto.Precio * i.Cantidad)),
                TopProductos = ticketsHoy
                    .SelectMany(t => t.Items)
                    .GroupBy(i => i.Producto.Nombre)
                    .Select(g => $"{g.Key} ({g.Sum(i => i.Cantidad)})")
                    .OrderByDescending(p => p)
                    .Take(10)
                    .ToList(),
                TopCategorias = ticketsHoy
                    .SelectMany(t => t.Items)
                    .GroupBy(i => i.Producto.Categoria)
                    .Select(g => $"{g.Key} ({g.Sum(i => i.Cantidad)})")
                    .OrderByDescending(c => c)
                    .Take(3)
                    .ToList()
            };

            return View(reporte);
        }
    }
}
