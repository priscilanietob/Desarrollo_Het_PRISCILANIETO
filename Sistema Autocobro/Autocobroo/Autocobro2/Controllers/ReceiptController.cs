using Microsoft.AspNetCore.Mvc;
using Autocobro.Models;

namespace Autocobro.Controllers
{
    public class ReceiptController : Controller
    {
        private readonly DataContext _context;
        public ReceiptController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            var ticket = _context.Tickets
                .Where(t => t.Id == id)
                .FirstOrDefault();

            if (ticket == null) return RedirectToAction("Index", "Home");
            return View(ticket);
        }
    }
}
