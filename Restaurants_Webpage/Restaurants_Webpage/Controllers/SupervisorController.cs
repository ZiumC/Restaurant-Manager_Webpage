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
        private readonly string _employeeDetailsUrl;
        private readonly string _employeeDeleteCertificateUrl;


        public SupervisorController(IConfiguration config)
        {
            _config = config;

            string employeesBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Employees"]);
            string employeeDetailsUrl = employeesBaseUrl + "/{0}";
            string employeeDeleteCertificateUrl = employeesBaseUrl + "/{0}" + _config["Endpoints:Paths:Certificate"] + "/{1}";

            try
            {
                if (string.IsNullOrEmpty(employeesBaseUrl))
                {
                    throw new Exception("Employees base url can't be empty");
                }

                if (string.IsNullOrEmpty(employeeDetailsUrl))
                {
                    throw new Exception("Employee details url can't be empty");
                }

                if (string.IsNullOrEmpty(employeeDeleteCertificateUrl))
                {
                    throw new Exception("Employee delete certificate url can't be empty");
                }

                _employeesUrl = employeesBaseUrl;
                _employeeDetailsUrl = employeeDetailsUrl;
                _employeeDeleteCertificateUrl = employeeDeleteCertificateUrl;

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

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> EmployeeForm(int idEmpployee)
        {
            if (idEmpployee > 0)
            {
                HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
                if (string.IsNullOrEmpty(jwtUtils.GetJwtCookie()))
                {
                    TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                    return RedirectToAction("index", "home");
                }
                string url = string.Format(_employeeDetailsUrl, idEmpployee);
                var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.GET, null, jwtUtils.GetJwtCookie());
                if (response == null)
                {
                    TempData["ActionFailed"] = "Unable connect to server the external server. You can't make a new reservation, please try again later.";
                    return RedirectToAction("index", "home");
                }

                var contentResponse = await response.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<EmployeeModel>(contentResponse);
                return View(employee);
            }

            return View();
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> DeleteCertificate(int idEmpployee, int idCertificate)
        {
            if (idEmpployee < 0 || idCertificate < 0)
            {
                TempData["ActionFailed"] = "Did you modified request?";
                return RedirectToAction("employees", "supervisor");
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("index", "home");
            }

            string url = string.Format(_employeeDeleteCertificateUrl, idEmpployee, idCertificate);
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.DELETE, null, jwtUtils.GetJwtCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server. You can't make a new reservation, please try again later.";
                return RedirectToAction("index", "home");
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["ActionSucceeded"] = $"Employee certificate has been deleted correctly!";
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
            }

            return RedirectToAction("employees", "supervisor");
        }

        public IActionResult MyRestaurant()
        {
            return View();
        }
    }
}
