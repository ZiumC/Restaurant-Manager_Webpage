using Microsoft.AspNetCore.Mvc;

namespace Restaurants_Webpage.Controllers
{
    public class OwnerController : Controller
    {
        public IActionResult Complaints()
        {
            return View();
        }
    }
}
