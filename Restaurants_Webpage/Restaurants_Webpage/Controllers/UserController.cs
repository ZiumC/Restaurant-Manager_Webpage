using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurants_Webpage.Models.UserModels.ClientModels;
using Restaurants_Webpage.Utils;
using System.IdentityModel.Tokens.Jwt;

namespace Restaurants_Webpage.Controllers
{
    public class UserController : Controller
    {

        private readonly IConfiguration _config;
        private readonly string _clientDataUrl;
        private readonly string _confirmReservationUrl;
        private readonly string _cancelReservationUrl;
        private readonly string _jwtCookieName;
        private readonly string _jwtCookieIdClientFieldName;

        public UserController(IConfiguration config)
        {
            _config = config;

            string clientDataBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Clients"]);
            string clientDataUrl = clientDataBaseUrl + "/{0}";

            string clientReservationBaseUrl = clientDataUrl + _config["Endpoints:Paths:Reservation"] + "/{1}";
            string confirmReservationUrl = string.Concat(clientReservationBaseUrl, _config["Endpoints:Paths:Confirm"]);
            string cancelReservationUrl = string.Concat(clientReservationBaseUrl, _config["Endpoints:Paths:Cancel"]);

            string jwtCookieName = _config["ApplicationSettings:JwtSettings:CookieSettings:CookieName"];
            string jwtCookieIdClientFieldName = _config["ApplicationSettings:UserSettings:CookieSettings:Client:IdName"];

            try
            {
                if (string.IsNullOrEmpty(clientDataUrl))
                {
                    throw new Exception("Client data url can't be empty");
                }

                if (string.IsNullOrEmpty(jwtCookieName))
                {
                    throw new Exception("Jwt cookie namne can't be empty");
                }

                if (string.IsNullOrEmpty(jwtCookieIdClientFieldName))
                {
                    throw new Exception("Cookie id client name can't be empty");
                }

                if (string.IsNullOrEmpty(confirmReservationUrl))
                {
                    throw new Exception("Confirm reservation url can't be empty");
                }

                if (string.IsNullOrEmpty(cancelReservationUrl))
                {
                    throw new Exception("Cancel reservation url can't be empty");
                }

                _clientDataUrl = clientDataUrl;
                _confirmReservationUrl = confirmReservationUrl;
                _cancelReservationUrl = cancelReservationUrl;
                _jwtCookieName = jwtCookieName;
                _jwtCookieIdClientFieldName = jwtCookieIdClientFieldName;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

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

        [Authorize(Roles = UserRolesUtility.Client)]
        public async Task<IActionResult> MyReservations()
        {
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

            string reservationUrl = string.Format(_clientDataUrl, cookieClientId);
            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {jwtCookie}"}
            };

            var response = await HttpRequestUtility.SendRequestAsync(reservationUrl, Utils.HttpMethods.GET, null, headers);
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server. You can't see your reservations. Please try again later.";
                return RedirectToAction("index", "home");
            }

            var contentResponse = await response.Content.ReadAsStringAsync();
            var clientDetails = JsonConvert.DeserializeObject<ClientDetailsModel>(contentResponse);

            return View(clientDetails);
        }

        public async Task<IActionResult> ModifyReservation(int idReservation, string actionType)
        {
            if (string.IsNullOrEmpty(actionType) || idReservation < 0) 
            {
                TempData["ActionFailed"] = "Did you modified request?";
                return RedirectToAction("myReservations", "user");
            }

            string actionUrl;
            if (actionType == "Confirm")
            {
                actionUrl = _confirmReservationUrl;
            }
            else
            {
                actionUrl = _cancelReservationUrl;
            }

            string? jwtCookie = HttpContext.Request.Cookies[_jwtCookieName];
            if (jwtCookie == null)
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("myReservations", "user");
            }

            var tokenContent = new JwtSecurityTokenHandler().ReadToken(jwtCookie) as JwtSecurityToken;
            string? cookieClientId = tokenContent?.Claims.First(claim => claim.Type == _jwtCookieIdClientFieldName).Value;
            if (string.IsNullOrEmpty(cookieClientId))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("myReservations", "user");
            }

            string url = string.Format(actionUrl, cookieClientId, idReservation);
            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {jwtCookie}"}
            };
            var response = await HttpRequestUtility.SendRequestAsync(url, Utils.HttpMethods.PUT, null, headers);
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server. You can't make a new reservation, please try again later.";
                return RedirectToAction("index", "home");
            }

            string? responseMessage = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                TempData["ActionSucceeded"] = "Reservation has been modified correctly.";
            }
            else if (!string.IsNullOrEmpty(responseMessage))
            {
                TempData["ActionFailed"] = responseMessage;
            }
            else
            {
                TempData["ActionFailed"] = "Unable to modify reservation.";
            }

            return RedirectToAction("myReservations", "user");
        }

        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
