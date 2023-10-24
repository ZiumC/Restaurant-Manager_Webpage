using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurants_Webpage.Models.ClientModels.Restaurant;
using Restaurants_Webpage.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

namespace Restaurants_Webpage.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly string _restaurantMenuUrl;
        private readonly string _makeReservationUrl;
        private readonly string _jwtCookieName;
        private readonly string _jwtCookieIdClientFieldName;
        private readonly IConfiguration _config;

        public RestaurantController(IConfiguration config)
        {
            _config = config;

            string restaurantBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Clients"]);
            string restaurantMenuUrl = string.Concat(restaurantBaseUrl, _config["Endpoints:Paths:Restaurant"]);
            string makeReservationUrl = string.Concat(restaurantBaseUrl + "/{0}", _config["Endpoints:Paths:Reservation"]);

            string jwtCookieName = _config["ApplicationSettings:JwtSettings:CookieSettings:CookieName"];
            string jwtCookieIdClientFieldName = _config["ApplicationSettings:UserSettings:CookieSettings:Client:IdName"];

            try
            {
                if (string.IsNullOrEmpty(restaurantMenuUrl))
                {
                    throw new Exception("Restaurant menu url can't be empty");
                }

                if (string.IsNullOrEmpty(makeReservationUrl))
                {
                    throw new Exception("Make reservation url can't be empty");
                }

                if (string.IsNullOrEmpty(jwtCookieName))
                {
                    throw new Exception("Jwt cookie namne can't be empty");
                }

                if (string.IsNullOrEmpty(jwtCookieIdClientFieldName))
                {
                    throw new Exception("Cookie id client name can't be empty");
                }

                _restaurantMenuUrl = restaurantMenuUrl;
                _makeReservationUrl = makeReservationUrl;
                _jwtCookieName = jwtCookieName;
                _jwtCookieIdClientFieldName = jwtCookieIdClientFieldName;

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

            string? jwtCookie = HttpContext.Request.Cookies[_jwtCookieName];
            if (jwtCookie == null)
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("index", "home");
            }

            var tokenContent = new JwtSecurityTokenHandler().ReadToken(jwtCookie) as JwtSecurityToken;
            string? cookieClientId = tokenContent?.Claims.First(claim => claim.Type == _jwtCookieIdClientFieldName).Value;

            if (string.IsNullOrEmpty(cookieClientId))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("index", "home");
            }


            var body = JsonContent.Create(new
            {
                reservationDate = reservationDate,
                howManyPeoples = howManyPeoples,
                idRestaurant = idRestaurant
            });

            string reservationUrl = string.Format(_makeReservationUrl, cookieClientId);
            var response = await HttpRequestUtility.SendRequestAsync(reservationUrl, Utils.HttpMethods.POST, body);
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server. You can't make a new reservation, please try again later.";
                return RedirectToAction("index", "home");
            }

            string? responseMessage = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                TempData["ActionSucceeded"] = "Reservation booked correctly.";
            }
            else if (!string.IsNullOrEmpty(responseMessage))
            {
                TempData["ActionFailed"] = responseMessage;
            }
            else
            {
                TempData["ActionFailed"] = "Unable to book an reservation.";
                return RedirectToAction("index", "home");
            }
        }
    }
}
