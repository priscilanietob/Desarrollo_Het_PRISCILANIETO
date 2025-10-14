using Microsoft.AspNetCore.Mvc;
using Autocobro.Models;

namespace Autocobro.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DataContext _context;
        public ProductsController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View(_context.Productos.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Productos.Add(producto);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(producto);
        }

        public IActionResult Delete(int id)
        {
            var prod = _context.Productos.Find(id);
            if (prod != null)
            {
                _context.Productos.Remove(prod);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
