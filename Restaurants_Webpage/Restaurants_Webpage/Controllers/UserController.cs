using Microsoft.AspNetCore.Mvc;

namespace Restaurants_Webpage.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register() 
        {
            return View();
        }

        public IActionResult Forbidden() 
        {
            return View();
        }
    }
}
