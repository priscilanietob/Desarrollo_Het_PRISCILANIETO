using Microsoft.AspNetCore.Mvc;

namespace Autocobro.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
