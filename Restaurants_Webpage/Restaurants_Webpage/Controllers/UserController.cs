using Microsoft.AspNetCore.Mvc;

namespace Restaurants_Webpage.Controllers
{
    public class UserController : Controller
    {

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            List<string> cookiesToDelete = new List<string>
            {
                "RefreshToken",
                "AccessToken"
            };

            foreach (var cookieName in cookiesToDelete)
            {
                HttpContext.Response.Cookies.Delete(cookieName);
            }

            return RedirectToAction("index", "home");
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
