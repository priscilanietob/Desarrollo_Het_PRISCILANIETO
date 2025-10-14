using Microsoft.AspNetCore.Mvc;
using Autocobro.Models;
using Microsoft.EntityFrameworkCore;

namespace Autocobro.Controllers
{
    public class ReportsController : Controller
    {
        private readonly DataContext _context;

        public ReportsController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Diario()
        {
            // Obtener los tickets del día actual
            var hoy = DateTime.Today;
            var ticketsHoy = _context.Tickets
                .Include(t => t.Items)
                .ThenInclude(i => i.Producto)
                .Where(t => t.Fecha.Date == hoy)
                .ToList();

            // Crear el objeto del reporte
            var reporte = new Reporte
            {
                VentasTotales = ticketsHoy.Count,
                IngresosTotales = ticketsHoy.Sum(t => t.Items.Sum(i => i.Producto.Precio * i.Cantidad)),
                IngresosEfectivo = ticketsHoy
                    .Where(t => t.MetodoPago == "Efectivo")
                    .Sum(t => t.Items.Sum(i => i.Producto.Precio * i.Cantidad)),
                IngresosTarjeta = ticketsHoy
                    .Where(t => t.MetodoPago == "Tarjeta")
                    .Sum(t => t.Items.Sum(i => i.Producto.Precio * i.Cantidad))
            };

            // Top 10 productos vendidos
            var productosVendidos = ticketsHoy
                .SelectMany(t => t.Items)
                .GroupBy(i => i.Producto.Nombre)
                .Select(g => new { Nombre = g.Key, Cantidad = g.Sum(i => i.Cantidad) })
                .OrderByDescending(p => p.Cantidad)
                .Take(10)
                .ToList();

            reporte.TopProductos = productosVendidos.Select(p => $"{p.Nombre} ({p.Cantidad})").ToList();

            // Top 3 categors vendidas
            var categoriasVendidas = ticketsHoy
                .SelectMany(t => t.Items)
                .GroupBy(i => i.Producto.Categoria)
                .Select(g => new { Categoria = g.Key, Cantidad = g.Sum(i => i.Cantidad) })
                .OrderByDescending(c => c.Cantidad)
                .Take(3)
                .ToList();

            reporte.TopCategorias = categoriasVendidas.Select(c => $"{c.Categoria} ({c.Cantidad})").ToList();

            return View("Reporte", reporte);
        }
    }
}
