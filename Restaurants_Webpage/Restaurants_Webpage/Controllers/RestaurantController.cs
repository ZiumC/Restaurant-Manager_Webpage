using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurants_Webpage.Models.UserModels.AdministrativeModels.ExtendedModels;
using Restaurants_Webpage.Models.UserModels.ClientModels.ClientRestaurantModels;
using Restaurants_Webpage.Utils;

namespace Restaurants_Webpage.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly string _restaurantMenuUrl;
        private readonly string _makeReservationUrl;
        private readonly string _jwtCookieIdClientFieldName;
        private readonly string _restaurantsUrl;
        private readonly string _restaurantDetailsUrl;
        private readonly string _ownerRole;
        private readonly IConfiguration _config;

        public RestaurantController(IConfiguration config)
        {
            _config = config;

            string ownerRole = _config["ApplicationSettings:AdministrativeRoles:Owner"].ToUpper();

            string restaurantClientsBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Clients"]);
            string restaurantMenuUrl = string.Concat(restaurantClientsBaseUrl, _config["Endpoints:Paths:Restaurant"]);
            string makeReservationUrl = string.Concat(restaurantClientsBaseUrl + "/{0}", _config["Endpoints:Paths:Reservation"]);

            string jwtCookieIdClientFieldName = _config["ApplicationSettings:UserSettings:CookieSettings:Client:IdName"];

            string restaurantsBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Restaurants"]);
            string restaurantDetailsUrl = restaurantsBaseUrl + "/{0}";

            try
            {
                if (string.IsNullOrEmpty(ownerRole))
                {
                    throw new Exception("Owner role can't be empty");
                }

                if (string.IsNullOrEmpty(restaurantMenuUrl))
                {
                    throw new Exception("Restaurant menu url can't be empty");
                }

                if (string.IsNullOrEmpty(makeReservationUrl))
                {
                    throw new Exception("Make reservation url can't be empty");
                }

                if (string.IsNullOrEmpty(jwtCookieIdClientFieldName))
                {
                    throw new Exception("Cookie id client name can't be empty");
                }

                if (string.IsNullOrEmpty(restaurantsBaseUrl))
                {
                    throw new Exception("Rstaurants base url can't be empty");
                }

                if (string.IsNullOrEmpty(restaurantDetailsUrl))
                {
                    throw new Exception("Rstaurants details url can't be empty");
                }

                _ownerRole = ownerRole;
                _restaurantMenuUrl = restaurantMenuUrl;
                _makeReservationUrl = makeReservationUrl;
                _jwtCookieIdClientFieldName = jwtCookieIdClientFieldName;
                _restaurantsUrl = restaurantsBaseUrl;
                _restaurantDetailsUrl = restaurantDetailsUrl;


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

            var response = await HttpRequestUtility.SendRequestAsync($"{_restaurantMenuUrl}/{idRestaurant}", Utils.HttpMethods.GET, null, null);
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


            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            string? cookieClientId = jwtUtils.GetJwtRequestCookieValue(_jwtCookieIdClientFieldName, jwtUtils.GetJwtRequestCookie());
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()) || string.IsNullOrEmpty(cookieClientId))
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
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(reservationUrl, Utils.HttpMethods.POST, body, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server. You can't make a new reservation, please try again later.";
                return RedirectToAction("index", "home");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["ActionSucceeded"] = "Reservation booked correctly.";
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }

            return RedirectToAction("index", "home");
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> Restaurants()
        {
            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return View();
            }

            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(_restaurantsUrl, Utils.HttpMethods.GET, null, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return View();
            }

            if (response.IsSuccessStatusCode)
            {
                string? userRole = jwtUtils.GetJwtRequestCookieValue(JwtFields.ROLE, jwtUtils.GetJwtRequestCookie());

                var restaurantsJsonData = await response.Content.ReadAsStringAsync();
                var restaurants = JsonConvert.DeserializeObject<IEnumerable<ExtendedRestaurantModel>>(restaurantsJsonData);

                if (_ownerRole.Equals(jwtUtils.GetJwtRequestCookieValue(JwtFields.ROLE, jwtUtils.GetJwtRequestCookie())))
                {
                    return View((restaurants, userRole));
                }

                string? idEmployee = jwtUtils.GetJwtRequestCookieValue(JwtFields.EMP_ID, jwtUtils.GetJwtRequestCookie());
                if (!string.IsNullOrEmpty(idEmployee))
                {
                    int parsedIdEmployee = int.Parse(idEmployee, 0);
                    if (parsedIdEmployee > 0)
                    {
                        var supervisorRestaurants = restaurants?
                            .Where(r => r.RestaurantWorkers.Any(rw => rw.IdEmployee == parsedIdEmployee))
                            .ToList().AsEnumerable();
                        return View((supervisorRestaurants, userRole));
                    }
                }
                TempData["ActionFailed"] = "Something went wrong, unable to display restaurants at the moment. Please contact with administrator.";
                return View();
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }

            return View();
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> DishForm(int idDish, int idRestaurant)
        {
            if (idRestaurant <= 0)
            {
                TempData["ActionFailed"] = "Did you modified request?";
                return View();
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            string? userRole = jwtUtils.GetJwtRequestCookieValue(JwtFields.ROLE, jwtUtils.GetJwtRequestCookie());
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("employees", "supervisor");
            }

            string url = string.Format(_restaurantDetailsUrl, idRestaurant);
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.GET, null, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return View();
            }

            if (response.IsSuccessStatusCode)
            {
                var restaurantsJsonData = await response.Content.ReadAsStringAsync();
                var restaurants = JsonConvert.DeserializeObject<ExtendedRestaurantModel>(restaurantsJsonData);

                return View((restaurants, idDish, userRole));
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }

            return View();
        }
    }
}
