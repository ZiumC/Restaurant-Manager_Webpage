using Microsoft.AspNetCore.Authorization;
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

            string restaurantBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Clients"]);
            string restaurantMenuUrl = string.Concat(restaurantBaseUrl, _config["Endpoints:Paths:Restaurant"]);

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
                TempData["MenuError"] = $"<b>ID</b> of restaurant is invalid.";
                TempData["MenuStatusCode"] = $"Status code: <strong>400</strong>";
            }

            var response = await HttpRequestUtility.SendRequestAsync($"{_restaurantMenuUrl}/{idRestaurant}", Utils.HttpMethods.GET, null);
            if (response == null)
            {
                TempData["MenuError"] = "Unable connect to server the external.";
                TempData["MenuStatusCode"] = $"Status code: <strong>503</strong>";
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
                    TempData["MenuError"] = "Intrnal server error.";
                    TempData["MenuStatusCode"] = "Status code: <strong>500</strong>";
                }
            }
            else
            {
                TempData["MenuError"] = response.RequestMessage;
                TempData["MenuStatusCode"] = $"Status code: <strong>{response.StatusCode}</strong>";
            }
            return View(menuModel);
        }

        [Authorize(Roles = UserRolesUtility.Client)]
        public IActionResult Reservation(int idRestaurant, string restaurantName)
        {
            if (idRestaurant <= 0)
            {
                TempData["ReservationError"] = $"<b>ID</b> of restaurant is invalid.";
                TempData["ReservationStatusCode"] = $"Status code: <strong>400</strong>";
            }

            return View((idRestaurant, restaurantName));
        }

        [Authorize(Roles = UserRolesUtility.Client)]
        public async Task<IActionResult> BookTable(int idRestaurant, string restaurantName, DateTime reservationDate, int howManyPeoples)
        {

            if (howManyPeoples <= 0)
            {
                TempData["ReservationErrorLabel"] = $"<b>Numer of peoples</b> is invalid. At least should be one person.";
                return View("~/Views/Restaurant/Reservation.cshtml", (idRestaurant, restaurantName));

            }

            if (reservationDate.Date < DateTime.Now.Date)
            {
                TempData["ReservationErrorLabel"] = $"<b>Reservation date</b> is invalid.";
                return View("~/Views/Restaurant/Reservation.cshtml", (idRestaurant, restaurantName));
            }

            if (idRestaurant <= 0)
            {
                TempData["ActionFailed"] = "ID of restaurant is broken. You have to make reservation again!";
                return RedirectToAction("index", "home");
            }



            TempData["ActionSucceeded"] = "Reservation booked correctly.";
            return RedirectToAction("index", "home");
        }
    }
}
