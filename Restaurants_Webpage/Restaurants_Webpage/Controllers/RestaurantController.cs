using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurants_Webpage.Models.ClientModels.Restaurant;
using Restaurants_Webpage.Utils;

namespace Restaurants_Webpage.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly string _restaurantMenuUrl;
        private readonly IConfiguration _config;

        public RestaurantController(IConfiguration config)
        {
            _config = config;

            string restaurantMenuUrl = _config["Endpoints:GET:Clients:RestaurantMenu"];

            try
            {
                if (string.IsNullOrEmpty(restaurantMenuUrl))
                {
                    throw new Exception("Restaurant menu url can't be empty");
                }

                _restaurantMenuUrl = restaurantMenuUrl;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public async Task<IActionResult> Menu(int idRestaurant)
        {
            if (idRestaurant <= 0)
            {
                TempData["UrlError"] = $"<b>ID</b> of restaurant is invalid.";
                TempData["UrlStatusCode"] = $"Status code: <strong>400</strong>";
            }

            var response = await HttpRequestUtility.SendRequestAsync($"{_restaurantMenuUrl}/{idRestaurant}", Utils.HttpMethods.GET, null);
            if (response == null)
            {
                TempData["UrlError"] = "Unable connect to server the external.";
                TempData["UrlStatusCode"] = $"Status code: <strong>503</strong>";
                return View();
            }

            RestaurantMenuModel? menuModel = null;
            string responseMessage = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    menuModel = JsonConvert.DeserializeObject<RestaurantMenuModel>(responseMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["UrlError"] = "Intrnal server error.";
                    TempData["UrlStatusCode"] = "Status code: <strong>500</strong>";
                }
            }
            else
            {
                TempData["UrlError"] = response.RequestMessage;
                TempData["UrlStatusCode"] = $"Status code: <strong>{response.StatusCode}</strong>";
            }
            return View(menuModel);
        }

        public IActionResult Reservation()
        {
            return View();
        }
    }
}
