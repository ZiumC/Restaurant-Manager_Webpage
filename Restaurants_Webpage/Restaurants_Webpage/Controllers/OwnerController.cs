using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurants_Webpage.Utils;

namespace Restaurants_Webpage.Controllers
{
    public class OwnerController : Controller
    {
        public readonly IConfiguration _config;
        public string _removeEmpFromRestaurantUrl { get; set; }

        public OwnerController(IConfiguration config)
        {
            _config = config;

            string restaurantBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Restaurants"]);
            string removeEmpFromRestaurantUrl = restaurantBaseUrl + "/{0}" + _config["Endpoints:Paths:Employee"] + "/{1}";
            try
            {
                if (string.IsNullOrEmpty(removeEmpFromRestaurantUrl))
                {
                    throw new Exception("Remove employee from restaurant Url can't be empty");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            _removeEmpFromRestaurantUrl = removeEmpFromRestaurantUrl;

        }

        public IActionResult Complaints()
        {
            return View();
        }

        [Authorize(Roles = UserRolesUtility.Owner)]
        public async Task<IActionResult> RemoveEmployeeFromRestaurant(int idEmployee, int idRestaurant)
        {

            if (idEmployee <= 0 || idRestaurant <= 0)
            {
                TempData["ActionFailed"] = "Did you modified request?";
                return RedirectToAction("restaurants", "restaurant");
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("restaurants", "restaurant");
            }

            string url = string.Format(_removeEmpFromRestaurantUrl, idRestaurant, idEmployee);
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.DELETE, null, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return RedirectToAction("restaurants", "restaurant");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["ActionSucceeded"] = $"Employee has been successfully removed from restaurant!";
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }

            return RedirectToAction("employees", "supervisor", new { idRestaurant });
        }
    }
}
