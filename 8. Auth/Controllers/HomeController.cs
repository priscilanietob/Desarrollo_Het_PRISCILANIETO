using Auth.Models;
using Auth.Services; 
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Auth.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthCheckFilter))]
        public IActionResult Privacy()
        {
            var user = HttpContext.Items["User"] as User;
            
            if (user != null)
            {
                ViewData["UserName"] = user.Name;
                ViewData["UserEmail"] = user.Email;
                ViewData["UserImageUrl"] = user.ProfileImageUrl; 
            }
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}