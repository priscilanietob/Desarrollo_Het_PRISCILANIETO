using Microsoft.AspNetCore.Mvc;
using Autocobro.Models;
using System.Linq;
using System.Collections.Generic;

namespace Autocobro.Controllers
{
    public class CheckoutController : Controller
    {
        public static List<ItemCarrito> Carrito { get; set; } = new();
        private readonly DataContext _context;

        public CheckoutController(DataContext context)
        {
            _context = context;
        }

        // Renderiza la vista con productos disponibles y carrito
        public IActionResult Index()
        {
            ViewBag.ProductosDisponibles = _context.Productos.ToList();
            return View(Carrito);
        }

        // Eliminar un producto del carrito
        [HttpPost]
        public IActionResult EliminarAjax([FromBody] string codigoDeBarras)
        {
            var item = Carrito.FirstOrDefault(c => c.Producto.CodigoDeBarras == codigoDeBarras);
            if (item != null)
                Carrito.Remove(item);

            var carritoJson = Carrito.Select(c => new
            {
                codigoDeBarras = c.Producto.CodigoDeBarras,
                nombre = c.Producto.Nombre,
                precio = c.Producto.Precio,
                cantidad = c.Cantidad,
                subtotal = c.Subtotal
            }).ToList();

            return Json(new { success = true, carrito = carritoJson });
        }

        public IActionResult Cancelar()
        {
            Carrito.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Pagar() => RedirectToAction("Index", "Payment");

        [HttpPost]
        public IActionResult AgregarAjax([FromBody] BusquedaCantidadRequest request)
        {
            if (string.IsNullOrEmpty(request.Busqueda) || request.Cantidad < 1)
                return Json(new { success = false });

            // Buscar por Código de Barras o Nombre
            var producto = _context.Productos
                .FirstOrDefault(p => p.CodigoDeBarras == request.Busqueda || p.Nombre == request.Busqueda);

            if (producto != null)
            {
                var existente = Carrito.FirstOrDefault(c => c.Producto.CodigoDeBarras == producto.CodigoDeBarras);
                if (existente != null)
                    existente.Cantidad += request.Cantidad; // Sumar cantidad
                else
                    Carrito.Add(new ItemCarrito { Producto = producto, Cantidad = request.Cantidad });
            }

            var carritoJson = Carrito.Select(c => new
            {
                codigoDeBarras = c.Producto.CodigoDeBarras,
                nombre = c.Producto.Nombre,
                precio = c.Producto.Precio,
                cantidad = c.Cantidad,
                subtotal = c.Subtotal
            }).ToList();

            return Json(new { success = true, carrito = carritoJson });
        }
    }

    // Clase para recibir JSON con cantidad
    public class BusquedaCantidadRequest
    {
        public string Busqueda { get; set; } = "";
        public int Cantidad { get; set; } = 1;
    }
}