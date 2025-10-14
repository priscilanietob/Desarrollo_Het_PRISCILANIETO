using Microsoft.AspNetCore.Mvc;
using Autocobro.Models;

namespace Autocobro.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DataContext _context;
        private static List<ItemCarrito> carrito = CheckoutController.Carrito; // acceso al carrito

        public PaymentController(DataContext context)
        {
            _context = context;
        }

        // Vista principal de pago
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Total = carrito.Sum(i => i.Subtotal);
            return View();
        }

        // Procesar pago en efectivo
        [HttpPost]
        public IActionResult Procesar(string metodo, decimal efectivoRecibido)
        {
            var total = carrito.Sum(i => i.Subtotal);
            decimal cambio = metodo == "Efectivo" ? efectivoRecibido - total : 0;

            // Crear ticket
            var ticket = new Ticket
            {
                Fecha = DateTime.Now,
                MetodoPago = metodo,
                Items = carrito.Select(c => new ItemCarrito
                {
                    Producto = c.Producto,
                    Cantidad = c.Cantidad
                }).ToList(),
                Cambio = cambio
            };

            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            // Limpiar carrito
            carrito.Clear();

            return RedirectToAction("Index", "Ticket", new { id = ticket.Id });
        }

        // Vista de pago con tarjeta
        [HttpGet]
        public IActionResult Tarjeta()
        {
            ViewBag.Total = carrito.Sum(i => i.Subtotal);
            return View();
        }

 [HttpPost]
public IActionResult ProcesarTarjeta(string numeroTarjeta, string pin)
{
    var total = carrito.Sum(i => i.Subtotal);

    // Validación básica
    if (string.IsNullOrWhiteSpace(numeroTarjeta) || numeroTarjeta.Length != 16 ||
        string.IsNullOrWhiteSpace(pin) || pin.Length != 4)
    {
        ViewBag.Error = "Número de tarjeta o PIN inválido.";
        ViewBag.Total = total;
        return View("Tarjeta");
    }

    // Crear ticket
    var ticket = new Ticket
    {
        Fecha = DateTime.Now,
        MetodoPago = "Tarjeta",
        Items = carrito.Select(c => new ItemCarrito
        {
            Producto = c.Producto,
            Cantidad = c.Cantidad
        }).ToList(),
        Cambio = 0
    };

    _context.Tickets.Add(ticket);
    _context.SaveChanges();

    // Limpiar carrito
    carrito.Clear();

    return RedirectToAction("Index", "Tickets", new { id = ticket.Id });
}


    }
}