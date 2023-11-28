using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurants_Webpage.Models.CommonModels;
using Restaurants_Webpage.Models.UserModels.EmployeeModels;
using Restaurants_Webpage.Utils;
using Restaurants_Webpage.Utils.Validator;

namespace Restaurants_Webpage.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _employeesUrl;
        private readonly string _employeeDetailsUrl;
        private readonly string _addNewEmployeeUrl;
        private readonly string _updateEmployeeUrl;
        private readonly string _addNewEmployeeCertificateUrl;
        private readonly string _updateEmployeeCertificateUrl;
        private readonly string _employeeDeleteCertificateUrl;
        private readonly string _restarantEmployeesUrl;


        public SupervisorController(IConfiguration config)
        {

            _config = config;

            string employeesBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Employees"]);

            string employeeDetailsUrl = employeesBaseUrl + "/{0}";
            string employeeDeleteCertificateUrl = employeesBaseUrl + "/{0}" + _config["Endpoints:Paths:Certificate"] + "/{1}";

            string addNewEmployeeUrl = employeesBaseUrl;
            string updateEmployeeUrl = employeesBaseUrl + "/{0}";

            string addNewEmployeeCertificateUrl = employeesBaseUrl + "/{0}" + _config["Endpoints:Paths:Certificate"];
            string updateEmployeeCertificateUrl = employeesBaseUrl + "/{0}" + _config["Endpoints:Paths:Certificate"] + "/{1}";

            string restarantEmployeesUrl = employeesBaseUrl + _config["Endpoints:Paths:Restaurant"] + "/{0}";

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

                if (string.IsNullOrEmpty(addNewEmployeeUrl))
                {
                    throw new Exception("Add new employee url can't be empty");
                }

                if (string.IsNullOrEmpty(updateEmployeeUrl))
                {
                    throw new Exception("Update employee url can't be empty");
                }

                if (string.IsNullOrEmpty(addNewEmployeeCertificateUrl))
                {
                    throw new Exception("Add new employee certificate url can't be empty");
                }

                if (string.IsNullOrEmpty(updateEmployeeCertificateUrl))
                {
                    throw new Exception("Update employee certificate url can't be empty");
                }

                if (string.IsNullOrEmpty(restarantEmployeesUrl))
                {
                    throw new Exception("Restaurant employess url can't be empty");
                }

                _employeesUrl = employeesBaseUrl;
                _employeeDetailsUrl = employeeDetailsUrl;
                _employeeDeleteCertificateUrl = employeeDeleteCertificateUrl;
                _addNewEmployeeUrl = addNewEmployeeUrl;
                _updateEmployeeUrl = updateEmployeeUrl;
                _addNewEmployeeCertificateUrl = addNewEmployeeCertificateUrl;
                _updateEmployeeCertificateUrl = updateEmployeeCertificateUrl;
                _restarantEmployeesUrl = restarantEmployeesUrl;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> Employees(int idRestaurant)
        {
            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("employees", "supervisor", new { idRestaurant });
            }

            string url = _employeesUrl;
            string? idRestaurantString = TempData["IdRestaurant"]?.ToString();
            if (idRestaurant > 0)
            {
                url = string.Format(_restarantEmployeesUrl, idRestaurant);
            }
            //needs to work on it but later
            //else if (idRestaurantString != null && !idRestaurantString.Equals("0"))
            //{
            //    url = string.Format(_restarantEmployeesUrl, idRestaurantString);
            //    idRestaurant = int.Parse(idRestaurantString);
            //}

            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.GET, null, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server. You can't make a new reservation, please try again later.";
                return RedirectToAction("employees", "supervisor", new { idRestaurant });
            }

            var contentResponse = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<IEnumerable<EmployeeModel>>(contentResponse);


            return View((employees, idRestaurant));
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> EmployeeForm(int idEmployee)
        {
            if (idEmployee > 0)
            {
                HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
                if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
                {
                    TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                    return RedirectToAction("employees", "supervisor");
                }
                string url = string.Format(_employeeDetailsUrl, idEmployee);
                var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.GET, null, jwtUtils.GetJwtRequestCookie());
                if (response == null)
                {
                    TempData["ActionFailed"] = "Unable connect to server the external server. You can't make a new reservation, please try again later.";
                    return RedirectToAction("employees", "supervisor");
                }

                var contentResponse = await response.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<EmployeeModel>(contentResponse);
                return View(employee);
            }


            return View();
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> DeleteCertificate(int idEmployee, int idCertificate)
        {
            if (idEmployee < 0 || idCertificate < 0)
            {
                TempData["ActionFailed"] = "Did you modified request?";
                return RedirectToAction("employees", "supervisor");
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("employees", "supervisor");
            }

            string url = string.Format(_employeeDeleteCertificateUrl, idEmployee, idCertificate);
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.DELETE, null, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server. You can't delete certificate now, please try again later.";
                return RedirectToAction("employees", "supervisor");
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

        /// <summary>
        /// This method adds and updates existing employee. This two actions are very similar to each other
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <param name="addressModel"></param>
        /// <returns></returns>
        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> SetEmployee(EmployeeModel employeeModel, CommonAddressModel addressModel, int idEmployee, int idRestaurant)
        {
            employeeModel.Address = addressModel;
            if (EmployeeValidator.IsDefectedEmployee(employeeModel, _config))
            {
                TempData["ActionFailed"] = "Employee data contains errors!";
                TempData["FormError"] = "Unable to save changes because form contains errors";
                return RedirectToAction("employeeForm", "supervisor");
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("employees", "supervisor", new { idRestaurant });
            }

            var method = Utils.HttpMethods.POST;
            string url = _addNewEmployeeUrl;
            if (idEmployee > 0)
            {
                url = string.Format(_updateEmployeeUrl, idEmployee);
                method = Utils.HttpMethods.PUT;
            }

            var body = JsonContent.Create(employeeModel);
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, method, body, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return RedirectToAction("employees", "supervisor", new { idRestaurant });
            }

            if (response.IsSuccessStatusCode)
            {
                string actionDone = idEmployee > 0 ? "updated" : "added";
                TempData["ActionSucceeded"] = $"Employee has been {actionDone}!";
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
                TempData["FormError"] = "Unable to save changes because form contains errors";
                return RedirectToAction("employeeForm", "supervisor");
            }

            return RedirectToAction("employees", "supervisor", new { idRestaurant });
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> CertificateForm(int idEmployee, int idCertificate)
        {
            if (idEmployee <= 0)
            {
                TempData["ActionFailed"] = "Did you modified request?";
                return RedirectToAction("employees", "supervisor");
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("employees", "supervisor");
            }

            string url = string.Format(_employeeDetailsUrl, idEmployee);
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, Utils.HttpMethods.GET, null, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return RedirectToAction("employees", "supervisor");
            }

            var contentResponse = await response.Content.ReadAsStringAsync();
            var employee = JsonConvert.DeserializeObject<EmployeeModel>(contentResponse);
            return View((employee, idCertificate));
        }

        [Authorize(Roles = UserRolesUtility.OwnerAndSupervisor)]
        public async Task<IActionResult> SetEmployeeCertificate(int idEmployee, int idCertificate, EmployeeCertificateModel certificate)
        {
            if (idEmployee <= 0)
            {
                TempData["ActionFailed"] = "Did you modified request?";
                return RedirectToAction("employees", "supervisor");
            }

            var method = Utils.HttpMethods.POST;
            string url = string.Format(_addNewEmployeeCertificateUrl, idEmployee);
            if (idCertificate > 0)
            {
                url = string.Format(_updateEmployeeCertificateUrl, idEmployee, idCertificate);
                method = Utils.HttpMethods.PUT;
            }

            HttpJwtUtility jwtUtils = new HttpJwtUtility(_config, HttpContext);
            if (string.IsNullOrEmpty(jwtUtils.GetJwtRequestCookie()))
            {
                TempData["ActionFailed"] = "Jwt is broken. Please logout and then login again!";
                return RedirectToAction("employees", "supervisor");
            }

            var body = JsonContent.Create(new
            {
                name = certificate.Name,
                expirationDate = certificate.ExpirationDate
            });
            var response = await HttpRequestUtility.SendSecureRequestJwtAsync(url, method, body, jwtUtils.GetJwtRequestCookie());
            if (response == null)
            {
                TempData["ActionFailed"] = "Unable connect to server the external server, please try again later.";
                return RedirectToAction("employees", "supervisor");
            }

            if (response.IsSuccessStatusCode)
            {
                string actionDone = idEmployee > 0 ? "updated" : "added";
                TempData["ActionSucceeded"] = $"Employee certificate has been {actionDone}!";
            }
            else
            {
                TempData["ActionFailed"] = await HttpRequestUtility.GetResponseMessage(response);
                TempData["FormError"] = "Unable to save changes because form contains errors";
                return RedirectToAction("certificateForm", "supervisor", new { idEmployee, idCertificate });
            }

            return RedirectToAction("certificateForm", "supervisor", new { idEmployee });
        }

    }
}
