using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurants_Webpage.Models.UserModels.EmployeeModels;
using Restaurants_Webpage.Utils;

namespace Restaurants_Webpage.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _employeesUrl;

        public SupervisorController(IConfiguration config)
        {
            _config = config;

            string employeesBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Employees"]);

            try
            {
                if (string.IsNullOrEmpty(employeesBaseUrl))
                {
                    throw new Exception("mployees base url can't be empty");
                }

                _employeesUrl = employeesBaseUrl;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> Employees()
        {
            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("index", "home");
            }

            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(_employeesUrl, Utils.HttpMethods.GET, null, jwtUtils.GetJwtCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server. You can't make a new reservation, please try again later.";
                return RedirectToAction("index", "home");
            }

            var contentResponse = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<IEnumerable<EmployeeModel>>(contentResponse);


            return View(employees);

        }

        public IActionResult MyRestaurant()
        {
            return View();
        }
    }
}
