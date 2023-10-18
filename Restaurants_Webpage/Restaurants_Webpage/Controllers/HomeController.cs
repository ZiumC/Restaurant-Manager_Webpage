using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurants_Webpage.Models;
using Restaurants_Webpage.Models.DataModels.Index;
using Restaurants_Webpage.Utils;
using System.Diagnostics;

namespace Restaurants_Webpage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly string _restaurantsListUrl;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            string restaurantsListUrl = _config["Endpoints:GET:Clients:AllRestaurants"];
            try
            {
                if (string.IsNullOrEmpty(restaurantsListUrl))
                {
                    throw new Exception("Get clients restaurants url can't be empty");
                }

                _restaurantsListUrl = restaurantsListUrl;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<IActionResult> Index()
        {
            var response = await HttpRequestUtility.SendRequestAsync(_restaurantsListUrl, Utils.HttpMethods.GET, null);
            if (response != null)
            {
                var contentResponse = await response.Content.ReadAsStringAsync();
                var restaurants = JsonConvert.DeserializeObject<IEnumerable<RestaurantIndexModel>>(contentResponse);

                return View(restaurants);
            }

            TempData["RestaurantsClientError"] = "<b>Due to unforeseen circumstances</b> You can't see any restaurants at the moment.";

            return View();
        }

        [Authorize(Roles = UserRolesUtility.Client)]
        public IActionResult Details()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}