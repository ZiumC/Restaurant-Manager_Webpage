using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Restaurants_Webpage.Models;
using Restaurants_Webpage.Utils;
using System.Diagnostics;

namespace Restaurants_Webpage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient httpClient = new HttpClient();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = UserRolesUtility.Client)]
        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Login(string login, string password) 
        {
            string loginUrl = "https://localhost:7042/api/Users/login";

            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod("POST"), loginUrl);
            requestMessage.Content = JsonContent.Create(new { loginOrEmail = login, password = password });
            HttpResponseMessage response = httpClient.SendAsync(requestMessage).Result;

            if (response.IsSuccessStatusCode)
            {
                string jsonData = await response.Content.ReadAsStringAsync();
                JwtModel? jwt = JsonConvert.DeserializeObject<JwtModel>(jsonData);
                if (jwt != null)
                {
                    HttpContext.Response.Cookies.Append("AccessToken", jwt.AccessToken);
                    HttpContext.Response.Cookies.Append("RefreshToken", jwt.RefreshToken);
                    return RedirectToAction("Privacy", "Home");   
                }
                else 
                {
                    //invalid login/password
                    return RedirectToAction("Index", "Home");
                }

            }
            else 
            {
                //handling unknown error
            }
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}