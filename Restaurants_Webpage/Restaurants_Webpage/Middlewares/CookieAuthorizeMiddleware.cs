using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Restaurants_Webpage.Models;
using System.Threading.Tasks;

namespace Restaurants_Webpage.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CookieAuthorizeMiddleware
    {
        private readonly RequestDelegate _next;

        public CookieAuthorizeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            string cookieName = "AccessToken";
            var authenticationCookie = httpContext.Request.Cookies[cookieName];

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
            return builder.UseMiddleware<CookieAuthorizeMiddleware>();
        }
    }
}
