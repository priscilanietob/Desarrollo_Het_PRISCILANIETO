using Microsoft.AspNetCore.Mvc;
using Autocobro.Models;
using System.Collections.Generic;
using System.Linq;

namespace Autocobro.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index()
        {
            var carritoReal = CheckoutController.Carrito;
            
            var total = carritoReal.Sum(i => i.Subtotal);
            
            var ticket = new Ticket
            {
                Id = 1,
                MetodoPago = "Efectivo", 
                Items = carritoReal,
                Cambio = 0 
            };

            return View(ticket);
        }
    }
}