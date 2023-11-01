using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurants_Webpage.Models.UserModels.ClientModels;
using Restaurants_Webpage.Utils;

namespace Restaurants_Webpage.Controllers
{
    public class UserController : Controller
    {

        private readonly IConfiguration _config;
        private readonly string _clientDataUrl;
        private readonly string _confirmReservationUrl;
        private readonly string _cancelReservationUrl;
        private readonly string _makeComplaintUrl;
        private readonly string _jwtCookieIdClientFieldName;

        public UserController(IConfiguration config)
        {
            _config = config;

            string clientDataBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Clients"]);
            string clientDataUrl = clientDataBaseUrl + "/{0}";

            string clientReservationBaseUrl = clientDataUrl + _config["Endpoints:Paths:Reservation"] + "/{1}";
            string confirmReservationUrl = string.Concat(clientReservationBaseUrl, _config["Endpoints:Paths:Confirm"]);
            string cancelReservationUrl = string.Concat(clientReservationBaseUrl, _config["Endpoints:Paths:Cancel"]);
            string makeComplaintUrl = string.Concat(clientReservationBaseUrl, _config["Endpoints:Paths:Complaint"]);



            string jwtCookieIdClientFieldName = _config["ApplicationSettings:UserSettings:CookieSettings:Client:IdName"];

            try
            {
                if (string.IsNullOrEmpty(clientDataUrl))
                {
                    throw new Exception("Client data url can't be empty");
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

                if (string.IsNullOrEmpty(makeComplaintUrl))
                {
                    throw new Exception("Make complaint url can't be empty");
                }

                _clientDataUrl = clientDataUrl;
                _confirmReservationUrl = confirmReservationUrl;
                _cancelReservationUrl = cancelReservationUrl;
                _jwtCookieIdClientFieldName = jwtCookieIdClientFieldName;
                _makeComplaintUrl = makeComplaintUrl;
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
            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            string? cookieClientId = jwtUtils.GetJwtCookieValue(_jwtCookieIdClientFieldName);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtCookie()) || string.IsNullOrEmpty(cookieClientId))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("index", "home");
            }

            string reservationUrl = string.Format(_clientDataUrl, cookieClientId);
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(reservationUrl, Utils.HttpMethods.GET, null, jwtUtils.GetJwtCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server. You can't see your reservations. Please try again later.";
                return RedirectToAction("index", "home");
            }

            var contentResponse = await response.Content.ReadAsStringAsync();
            var clientDetails = JsonConvert.DeserializeObject<ClientDetailsModel>(contentResponse);

            return View(clientDetails);
        }

        [Authorize(Roles = UserRolesUtility.Client)]
        public async Task<IActionResult> ModifyReservationStatus(int idReservation, string actionType)
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

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            string? cookieClientId = jwtUtils.GetJwtCookieValue(_jwtCookieIdClientFieldName);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtCookie()) || string.IsNullOrEmpty(cookieClientId))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("index", "home");
            }

            string url = string.Format(actionUrl, cookieClientId, idReservation);
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.PUT, null, jwtUtils.GetJwtCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server. You can't make a new reservation, please try again later.";
                return RedirectToAction("myReservations", "user");
            }

            if (response.IsSuccessStatusCode)
            {
                string actionMade = actionType == "Confirm" ? "confirmed" : "cancelled";
                TempData["ActionSucceeded"] = $"Reservation has been {actionMade} correctly!";
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }

            return RedirectToAction("myReservations", "user");
        }

        [Authorize(Roles = UserRolesUtility.Client)]
        public async Task<IActionResult> MakeComplaint(int idReservation, string message)
        {
            if (idReservation < 0)
            {
                TempData["ActionFailed"] = "Did you modified request?";
                return RedirectToAction("myReservations", "user");
            }

            if (string.IsNullOrEmpty(message)) 
            {
                TempData["ActionFailed"] = "Message can't be empty.";
                return RedirectToAction("myReservations", "user");
            }

            if (message.Length <= 0 || message.Length > 350)
            {
                TempData["ActionFailed"] = "Message should contain 1-350 characters.";
                return RedirectToAction("myReservations", "user");
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            string? cookieClientId = jwtUtils.GetJwtCookieValue(_jwtCookieIdClientFieldName);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtCookie()) || string.IsNullOrEmpty(cookieClientId))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("index", "home");
            }

            var body = JsonContent.Create(new
            {
                message = message
            });
            string url = string.Format(_makeComplaintUrl, cookieClientId, idReservation);
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.POST, body, jwtUtils.GetJwtCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server. You can't make a new reservation, please try again later.";
                return RedirectToAction("index", "home");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["ActionSucceeded"] = "Complaint has been made correctly.";
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }
            return RedirectToAction("myReservations", "user");
        }

        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
