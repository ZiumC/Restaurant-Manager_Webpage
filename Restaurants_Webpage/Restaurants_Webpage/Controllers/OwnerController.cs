using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurants_Webpage.Models.UserModels.AdministrativeModels.BasicModels;
using Restaurants_Webpage.Models.UserModels.AdministrativeModels.ExtendedModels;
using Restaurants_Webpage.Models.UserModels.EmployeeModels;
using Restaurants_Webpage.Utils;

namespace Restaurants_Webpage.Controllers
{
    public class OwnerController : Controller
    {
        public readonly IConfiguration _config;
        private readonly string _restaurantsUrl;
        private readonly string _employeeDataUrl;
        private readonly string _employeeTypesUrl;
        private readonly string _removeEmpFromRestaurantUrl;

        public OwnerController(IConfiguration config)
        {
            _config = config;

            string restaurantBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Restaurants"]);
            string employeeBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Employees"]);

            string removeEmpFromRestaurantUrl = restaurantBaseUrl + "/{0}" + _config["Endpoints:Paths:Employee"] + "/{1}";
            string employeeDataUrl = employeeBaseUrl + "/{0}";
            string employeeTypesUrl = restaurantBaseUrl + _config["Endpoints:Paths:Employee"] + _config["Endpoints:Paths:Types"];

            try
            {
                if (string.IsNullOrEmpty(restaurantBaseUrl))
                {
                    throw new Exception("Restaurant base url can't be empty");
                }

                if (string.IsNullOrEmpty(employeeBaseUrl))
                {
                    throw new Exception("Employee base url can't be empty");
                }

                if (string.IsNullOrEmpty(removeEmpFromRestaurantUrl))
                {
                    throw new Exception("Remove employee from restaurant url can't be empty");
                }

                if (string.IsNullOrEmpty(employeeDataUrl))
                {
                    throw new Exception("Remove employee from restaurant url can't be empty");
                }

                if (string.IsNullOrEmpty(employeeTypesUrl))
                {
                    throw new Exception("Employee types url can't be empty");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            _restaurantsUrl = restaurantBaseUrl;
            _removeEmpFromRestaurantUrl = removeEmpFromRestaurantUrl;
            _employeeDataUrl = employeeDataUrl;
            _employeeTypesUrl = employeeTypesUrl;

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

        [Authorize(Roles = UserRolesUtility.Owner)]
        public async Task<IActionResult> Employment(int idEmployee)
        {
            if (idEmployee <= 0)
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

            var restaurantsResponse =
                await HttpRequestUtility.SendSecureRequestJwtAsync(_restaurantsUrl, Utils.HttpMethods.GET, null, jwtUtils.GetJwtRequestCookie());

            string employeeUrl = string.Format(_employeeDataUrl, idEmployee);
            var employeeResponse =
                await HttpRequestUtility.SendSecureRequestJwtAsync(employeeUrl, Utils.HttpMethods.GET, null, jwtUtils.GetJwtRequestCookie());

            var employeesTypesResponse =
                await HttpRequestUtility.SendSecureRequestJwtAsync(_employeeTypesUrl, Utils.HttpMethods.GET, null, jwtUtils.GetJwtRequestCookie());

            if (restaurantsResponse == null || employeeResponse == null || employeesTypesResponse == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return RedirectToAction("restaurants", "restaurant");
            }



            if (restaurantsResponse.IsSuccessStatusCode && employeeResponse.IsSuccessStatusCode && employeesTypesResponse.IsSuccessStatusCode)
            {
                var restaurantContentResponse = await restaurantsResponse.Content.ReadAsStringAsync();
                var employeeContentResponse = await employeeResponse.Content.ReadAsStringAsync();
                var employeesContentResponse = await employeesTypesResponse.Content.ReadAsStringAsync();

                var restaurants = JsonConvert.DeserializeObject<IEnumerable<ExtendedRestaurantModel>>(restaurantContentResponse);
                var employee = JsonConvert.DeserializeObject<EmployeeModel>(employeeContentResponse);
                var types = JsonConvert.DeserializeObject<IEnumerable<BasicEmployeeTypesModel>>(employeesContentResponse);

                return View((restaurants, employee, types));

            }
            else
            {
                TempData["ActionFailed"] = HttpRequestUtility._defaultResponseMessage;
                return RedirectToAction("employees", "supervisor");
            }
        }
    }
}
