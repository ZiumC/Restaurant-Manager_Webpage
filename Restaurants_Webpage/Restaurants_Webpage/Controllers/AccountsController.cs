using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurants_Webpage.Models;
using Restaurants_Webpage.Utils;
using System.Net;
using System.Text.RegularExpressions;

namespace Restaurants_Webpage.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly int _loginMinLength;
        private readonly int _loginMaxLength;
        private readonly int _passMinLength;
        private readonly int _passMaxLength;
        private readonly string _emailRegex;
        private readonly string _loginRegex;
        private readonly string _peselRegex;
        private readonly string _loginUrl;
        private readonly string _registerUrl;

        public AccountsController(IConfiguration config)
        {
            _config = config;

            string loginMinLength = _config["ApplicationSettings:UserSettings:Login:MinLength"];
            string loginMaxLength = _config["ApplicationSettings:UserSettings:Login:MaxLength"];
            string passMinLength = _config["ApplicationSettings:UserSettings:Password:MinLength"];
            string passMaxLength = _config["ApplicationSettings:UserSettings:Password:MaxLength"];

            string emailRegex = _config["ApplicationSettings:DataValidation:EmailRegex"];
            string loginRegex = _config["ApplicationSettings:DataValidation:LoginRegex"];
            string peselRegex = _config["ApplicationSettings:DataValidation:PeselRegex"];

            string userBaseUrl = string.Concat(_config["Endpoints:BaseHost"], _config["Endpoints:Controller:Users"]);


            string loginUrl = string.Concat(userBaseUrl, _config["Endpoints:Paths:Login"]);
            string registerUrl = string.Concat(userBaseUrl, _config["Endpoints:Paths:Register"]);

            try
            {
                if (string.IsNullOrEmpty(loginMinLength) || string.IsNullOrEmpty(loginMaxLength))
                {
                    throw new Exception("Login length can't be empty");
                }

                if (string.IsNullOrEmpty(passMinLength) || string.IsNullOrEmpty(passMaxLength))
                {
                    throw new Exception("Password length can't be empty");
                }

                if (string.IsNullOrEmpty(emailRegex))
                {
                    throw new Exception("Email regex can't be empty");
                }

                if (string.IsNullOrEmpty(loginRegex))
                {
                    throw new Exception("Login regex can't be empty");
                }

                if (string.IsNullOrEmpty(peselRegex))
                {
                    throw new Exception("Pesel regex can't be empty");
                }

                if (string.IsNullOrEmpty(loginUrl))
                {
                    throw new Exception("Login url can't be empty");
                }
                if (string.IsNullOrEmpty(registerUrl))
                {
                    throw new Exception("Register url can't be empty");
                }

                _loginMinLength = int.Parse(loginMinLength);
                _loginMaxLength = int.Parse(loginMaxLength);
                _passMinLength = int.Parse(passMinLength);
                _passMaxLength = int.Parse(passMaxLength);

                _emailRegex = emailRegex;
                _loginRegex = loginRegex;
                _peselRegex = peselRegex;

                _loginUrl = loginUrl;
                _registerUrl = registerUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<IActionResult> Login(string login, string password)
        {

            if (login.Length < _loginMinLength || login.Length > _loginMaxLength)
            {
                TempData["LoginError"] = $"<b>Login</b> should contains at least " +
                    $"<b>{_loginMinLength}</b> characters and max <b>{_loginMaxLength}</b> characters.";
                return RedirectToAction("login", "user");
            }

            if (password.Length < _passMinLength || password.Length > _passMaxLength)
            {
                TempData["LoginError"] = $"<b>Password</b> should contains at least " +
                    $"<b>{_passMinLength}</b> characters and max <b>{_passMaxLength}</b> characters.";
                return RedirectToAction("login", "user");
            }

            var body = JsonContent.Create(new { loginOrEmail = login, password = password });
            var response = await HttpRequestUtility.SendRequestAsync(_loginUrl, Utils.HttpMethods.POST, body);

            if (response == null)
            {
                TempData["LoginError"] = "<b>Unable connect to server</b>. You can't login to the system.";
                return RedirectToAction("login", "user");
            }

            string responseMessage = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                string jsonData = await response.Content.ReadAsStringAsync();
                JwtModel? jwt = JsonConvert.DeserializeObject<JwtModel>(jsonData);
                if (jwt != null)
                {
                    HttpContext.Response.Cookies.Append("AccessToken", jwt.AccessToken);
                    HttpContext.Response.Cookies.Append("RefreshToken", jwt.RefreshToken);
                    TempData["LoginError"] = null;
                    return RedirectToAction("details", "home");
                }
                else
                {
                    TempData["LoginError"] = "Unable to receive an JWT";
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (!string.IsNullOrEmpty(responseMessage))
                {
                    TempData["LoginError"] = responseMessage;
                }

            }
            else if (!string.IsNullOrEmpty(responseMessage))
            {
                TempData["LoginError"] = await response.Content.ReadAsStringAsync();
            }
            return RedirectToAction("login", "user");
        }

        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (model.login.Length < _loginMinLength || model.login.Length > _loginMaxLength)
            {
                TempData["RegisterError"] = $"<b>Login</b> should contains at least " +
                    $"<b>{_loginMinLength}</b> characters and max <b>{_loginMaxLength}</b> characters.";
                return RedirectToAction("register", "user");
            }

            if (!Regex.Match(model.login, _loginRegex, RegexOptions.IgnoreCase).Success)
            {
                TempData["RegisterError"] = $"<b>Login</b> is invalid.";
                return RedirectToAction("register", "user");
            }

            if (model.email.Length < _loginMinLength || model.email.Length > _loginMaxLength)
            {
                TempData["RegisterError"] = $"<b>Email</b> should contains at least " +
                    $"<b>{_loginMinLength}</b> characters and max <b>{_loginMaxLength}</b> characters.";
                return RedirectToAction("register", "user");
            }

            if (!Regex.Match(model.email, _emailRegex, RegexOptions.IgnoreCase).Success)
            {
                TempData["RegisterError"] = "<b>Email</b> is invalid.";
                return RedirectToAction("register", "user");
            }

            if (model.password1.Length < _passMinLength || model.password1.Length > _passMaxLength)
            {
                TempData["RegisterError"] = $"<b>Password</b> should contains at least " +
                    $"<b>{_passMinLength}</b> characters and max <b>{_passMaxLength}</b> characters.";
                return RedirectToAction("register", "user");
            }

            if (!model.password1.Equals(model.password2))
            {
                TempData["RegisterError"] = "<b>Passwords</b> aren't equal.";
                return RedirectToAction("register", "user");
            }

            var registerEmployee = model.registerMeAsEmployee;
            if (registerEmployee != null && registerEmployee.Equals("on"))
            {
                if (!Regex.Match(model.pesel, _peselRegex, RegexOptions.IgnoreCase).Success)
                {
                    TempData["RegisterError"] = $"<b>PESEL</b> is invalid.";
                    return RedirectToAction("register", "user");
                }

                if ((DateTime.Now.Date - model.hiredDate).TotalDays / 365 >= 100)
                {
                    TempData["RegisterError"] = $"<b>Hired date</b> is invalid.";
                    return RedirectToAction("register", "user");
                }
            }

            var body = JsonContent.Create(new
            {
                login = model.login,
                email = model.email,
                password = model.password1,
                registerMeAsEmployee = registerEmployee != null && registerEmployee.Equals("on") ? true : false,
                pesel = model.pesel,
                hiredDate = model.hiredDate
            });
            var response = await HttpRequestUtility.SendRequestAsync(_registerUrl, Utils.HttpMethods.POST, body);

            if (response == null)
            {
                TempData["RegisterError"] = "<b>Unable connect to server</b>. You can't register to the system.";
                return RedirectToAction("register", "user");
            }

            string responseMessage = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Correctly registered!";
                TempData["RegisterError"] = null;
                return RedirectToAction("index", "home");
            }
            else if (!string.IsNullOrEmpty(responseMessage))
            {
                TempData["RegisterError"] = responseMessage;
            }
            else
            {
                TempData["RegisterError"] = "Unable to register an new account";
            }
            return RedirectToAction("register", "user");

        }
    }
}
