using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Restaurants_Webpage.Models;
using System.Threading.Tasks;

namespace Restaurants_Webpage.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthorizeCookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _cookieName = "AccessToken";

        public AuthorizeCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var authenticationCookie = httpContext.Request.Cookies[_cookieName];

            Console.WriteLine(authenticationCookie);

            if (authenticationCookie != null)
            {
                httpContext.Request.Headers.Append("Authorization", "Bearer " + authenticationCookie);
            }

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CookieAuthorizeMiddlewareExtensions
    {
        public static IApplicationBuilder UseCookieAuthorizeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorizeCookieMiddleware>();
        }
    }
}
