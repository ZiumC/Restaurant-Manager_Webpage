using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurants_Webpage.Models.CommonModels;
using Restaurants_Webpage.Models.UserModels.AdministrativeModels.BasicModels;
using Restaurants_Webpage.Models.UserModels.AdministrativeModels.ExtendedModels;
using Restaurants_Webpage.Models.UserModels.ClientModels.ClientRestaurantModels;
using Restaurants_Webpage.Utils;
using Restaurants_Webpage.Utils.Validator;
using System;

namespace Restaurants_Webpage.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly string _restaurantMenuUrl;
        private readonly string _makeReservationUrl;
        private readonly string _jwtCookieIdClientFieldName;
        private readonly string _restaurantsUrl;
        private readonly string _restaurantDetailsUrl;
        private readonly string _restaurantDishesUrl;
        private readonly string _restaurantDishUrl;
        private readonly string _restaurantDishDetailsUrl;
        private readonly string _restaurantRemoveDishUrl;
        private readonly string _restaurantStatsUrl;
        private readonly string _ownerRole;
        private readonly IConfiguration _config;

        public RestaurantController(IConfiguration config)
        {
            _config = config;

            string ownerRole = _config["ApplicationSettings:AdministrativeRoles:Owner"].ToUpper();

            string restaurantClientsBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Clients"]);
            string restaurantMenuUrl = string.Concat(restaurantClientsBaseUrl, _config["Endpoints:Paths:Restaurants"]);
            string makeReservationUrl = string.Concat(restaurantClientsBaseUrl + "/{0}", _config["Endpoints:Paths:Reservation"]);

            string jwtCookieIdClientFieldName = _config["ApplicationSettings:UserSettings:CookieSettings:Client:IdName"];

            string restaurantsBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Restaurants"]);
            string restaurantDetailsUrl = restaurantsBaseUrl + "/{0}";
            string restaurantDishesUrl = restaurantsBaseUrl + _config["Endpoints:Paths:Dishes"];
            string restaurantDishUrl = restaurantsBaseUrl + _config["Endpoints:Paths:Dish"];
            string restaurantDishDetailsUrl = restaurantDishUrl + "/{0}";
            string restaurantStatsUrl = restaurantsBaseUrl + _config["Endpoints:Paths:Stats"];

            string restaurantRemoveDishUrl = restaurantsBaseUrl + "/{0}" + _config["Endpoints:Paths:Dish"] + "/{1}";


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

                if (string.IsNullOrEmpty(restaurantDishesUrl))
                {
                    throw new Exception("Rstaurants dishes url can't be empty");
                }

                if (string.IsNullOrEmpty(restaurantDishUrl))
                {
                    throw new Exception("Rstaurants dishes url can't be empty");
                }

                if (string.IsNullOrEmpty(restaurantRemoveDishUrl))
                {
                    throw new Exception("Rstaurant remove dish url can't be empty");
                }

                if (string.IsNullOrEmpty(restaurantDishDetailsUrl))
                {
                    throw new Exception("Rstaurant dish details url can't be empty");
                }

                if (string.IsNullOrEmpty(restaurantStatsUrl))
                {
                    throw new Exception("Rstaurant stats url can't be empty");
                }

                _ownerRole = ownerRole;
                _restaurantMenuUrl = restaurantMenuUrl;
                _makeReservationUrl = makeReservationUrl;
                _jwtCookieIdClientFieldName = jwtCookieIdClientFieldName;
                _restaurantsUrl = restaurantsBaseUrl;
                _restaurantDetailsUrl = restaurantDetailsUrl;
                _restaurantDishesUrl = restaurantDishesUrl;
                _restaurantDishUrl = restaurantDishUrl;
                _restaurantRemoveDishUrl = restaurantRemoveDishUrl;
                _restaurantDishDetailsUrl = restaurantDishDetailsUrl;
                _restaurantStatsUrl = restaurantStatsUrl;

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
        public async Task<IActionResult> DishForm(int idRestaurant, int idDish)
        {
            if (idRestaurant <= 0 && idDish <= 0)
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
            if (idDish > 0)
            {
                url = string.Format(_restaurantDishDetailsUrl, idDish);
            }

            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.GET, null, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return View();
            }

            if (response.IsSuccessStatusCode)
            {
                if (idDish > 0)
                {
                    var dish = await HttpRequestUtility.DeserializeResponse<BasicDishModel>(response);
                    return View((new ExtendedRestaurantModel(), new List<ExtendedDishModel>().AsEnumerable(), dish));
                }
                else
                {

                    var restaurantsJsonData = await response.Content.ReadAsStringAsync();
                    var restaurant = JsonConvert.DeserializeObject<ExtendedRestaurantModel>(restaurantsJsonData);

                    var dishResponse = await HttpRequestUtility
                        .SendSecureRequestJwtAsync(_restaurantDishesUrl, Utils.HttpMethods.GET, null, jwtUtils.GetJwtRequestCookie());

                    if (dishResponse != null)
                    {
                        var dishes = await HttpRequestUtility.DeserializeResponse<IEnumerable<ExtendedDishModel>>(dishResponse);
                        var notAssignedDishesToRestaurant = dishes?
                            .Where(d => d.Restaurants.All(r => !r.Equals(restaurant?.Name)))
                            .ToList()
                            .AsEnumerable();

                        return View((restaurant, notAssignedDishesToRestaurant, new BasicDishModel()));
                    }

                    return View((restaurant, new List<ExtendedDishModel>().AsEnumerable(), new BasicDishModel()));
                }
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }

            return View();
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> AddDishToRestaurant(int idRestaurant, BasicDishModel dish)
        {
            if (idRestaurant <= 0)
            {
                TempData["ActionFailed"] = "Did you modified request?";
                return RedirectToAction("restaurants", "restaurant");
            }

            if (DishValidator.IsDefectedDish(dish))
            {
                TempData["FormError"] = "Dish data is invalid";
                return RedirectToAction("dishForm", "restaurant", new { idRestaurant });
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("employees", "supervisor");
            }

            var body = JsonContent.Create(new
            {
                name = dish.Name,
                price = dish.Price,
                idRestaurants = new List<int> { idRestaurant }
            });
            var response = await HttpRequestUtility
                .SendSecureRequestJwtAsync(_restaurantDishUrl, Utils.HttpMethods.POST, body, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return RedirectToAction("restaurants", "restaurant");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["ActionSucceeded"] = $"Dish has been added to restaurant!";
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }

            return RedirectToAction("dishForm", "restaurant", new { idRestaurant });
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> RemoveDishFromRestaurant(int idRestaurant, int idDish)
        {
            if (idRestaurant <= 0 || idDish <= 0)
            {
                TempData["ActionFailed"] = "Did you modified request?";
                return RedirectToAction("restaurants", "restaurant");
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("employees", "supervisor");
            }

            string url = string.Format(_restaurantRemoveDishUrl, idRestaurant, idDish);
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.DELETE, null, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return RedirectToAction("restaurants", "restaurant");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["ActionSucceeded"] = $"Dish has been removed from restaurant!";
                return RedirectToAction("dishForm", "restaurant", new { idRestaurant });
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }

            return RedirectToAction("restaurants", "restaurant");
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> AssignDishToRestaurant(int idRestaurant, int idDish)
        {
            if (idRestaurant <= 0 || idDish <= 0)
            {
                TempData["ActionFailed"] = "Did you modified request?";
                return RedirectToAction("restaurants", "restaurant");
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("employees", "supervisor");
            }

            string url = string.Format(_restaurantRemoveDishUrl, idRestaurant, idDish);
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.POST, null, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return RedirectToAction("restaurants", "restaurant");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["ActionSucceeded"] = $"Dish has been assigned to restaurant!";
                return RedirectToAction("dishForm", "restaurant", new { idRestaurant });
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }

            return RedirectToAction("restaurants", "restaurant");
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> SetDishData(BasicDishModel dishData)
        {
            if (dishData == null)
            {
                TempData["FormError"] = "Dish data is empty";
                return RedirectToAction("restaurants", "restaurant");
            }

            if (string.IsNullOrEmpty(dishData.Name))
            {
                TempData["FormError"] = "Dish name can't be empty";
                return RedirectToAction("dishForm", "restaurant", new { dishData.IdDish });
            }

            if (dishData.Price < 0)
            {
                TempData["FormError"] = "Dish price is invalid";
                return RedirectToAction("dishForm", "restaurant", new { dishData.IdDish });
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("employees", "supervisor");
            }

            string url = string.Format(_restaurantDishDetailsUrl, dishData.IdDish);

            var body = JsonContent.Create(new
            {
                name = dishData.Name,
                price = dishData.Price
            });

            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.PUT, body, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return RedirectToAction("restaurants", "restaurant");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["ActionSucceeded"] = $"Dish data has been modified!";
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }

            return RedirectToAction("restaurants", "restaurant");
        }

        [Authorize(Roles = UserRolesUtility.Owner)]
        public async Task<IActionResult> RestaurantForm(int idRestaurant)
        {

            if (idRestaurant > 0)
            {
                HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
                if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
                {
                    TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                    return RedirectToAction("restaurant", "restaurants");
                }

                string url = string.Format(_restaurantDetailsUrl, idRestaurant);
                var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.GET, null, jwtUtils.GetJwtRequestCookie());
                if (response != null && response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var restaurantDetails = JsonConvert.DeserializeObject<ExtendedRestaurantModel>(responseContent);

                    if (restaurantDetails != null)
                    {
                        return View(new BasicRestaurantModel
                        {
                            IdRestaurant = restaurantDetails.IdRestaurant,
                            Name = restaurantDetails.Name,
                            Status = restaurantDetails.Status,
                            BonusBudget = restaurantDetails.BonusBudget,
                            Address = restaurantDetails.Address
                        });
                    }
                }
            }

            return View();
        }

        [Authorize(Roles = UserRolesUtility.Owner)]
        public async Task<IActionResult> SetRestaurantData(BasicRestaurantModel restaurantData, CommonAddressModel restaurantAddressData)
        {
            var onActionFailed = RedirectToAction("restaurantForm", "restaurant");
            if (restaurantData.IdRestaurant > 0)
            {
                onActionFailed = RedirectToAction("restaurantForm", "restaurant", new { restaurantData.IdRestaurant });
            }

            if (string.IsNullOrEmpty(restaurantData.Name))
            {
                TempData["FormError"] = "Restaurant name is empty";
                return onActionFailed;
            }

            if (string.IsNullOrEmpty(restaurantData.Status))
            {
                TempData["FormError"] = "Restaurant status is empty";
                return onActionFailed;
            }

            if (string.IsNullOrEmpty(restaurantAddressData.City))
            {
                TempData["FormError"] = "Restaurant address city is empty";
                return onActionFailed;
            }

            if (string.IsNullOrEmpty(restaurantAddressData.Street))
            {
                TempData["FormError"] = "Restaurant address street is empty";
                return onActionFailed;
            }

            if (string.IsNullOrEmpty(restaurantAddressData.BuildingNumber))
            {
                TempData["FormError"] = "Restaurant address building number is empty";
                return onActionFailed;
            }

            if (restaurantData.BonusBudget < 0)
            {
                TempData["FormError"] = "Restaurant bonus budget is invalid";
                return onActionFailed;
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("restaurant", "restaurants");
            }

            var body = JsonContent.Create(new BasicRestaurantModel
            {
                Name = restaurantData.Name,
                Status = restaurantData.Status,
                BonusBudget = restaurantData.BonusBudget,
                Address = restaurantAddressData
            });

            string url = _restaurantsUrl;
            Utils.HttpMethods method = Utils.HttpMethods.POST;
            if (restaurantData.IdRestaurant > 0)
            {
                url = _restaurantsUrl + $"/{restaurantData.IdRestaurant}";
                method = Utils.HttpMethods.PUT;
            }

            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, method, body, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return RedirectToAction("restaurants", "restaurant");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["ActionSucceeded"] = $"Changes has been saved successfully!";
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }

            return RedirectToAction("restaurants", "restaurant");
        }

        [Authorize(Roles = UserRolesUtility.Owner)]
        public async Task<IActionResult> RestaurantStats()
        {
            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("restaurants", "restaurant");
            }

            var response =
                await HttpRequestUtility.SendSecureRequestJwtAsync(_restaurantStatsUrl, Utils.HttpMethods.GET, null, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return RedirectToAction("restaurants", "restaurant");
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var stats = JsonConvert.DeserializeObject<IEnumerable<ExtendedRestaurantStats>>(responseContent);
                return View(stats);
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
                return RedirectToAction("restaurants", "restaurant");
            }

        }
    }
}
