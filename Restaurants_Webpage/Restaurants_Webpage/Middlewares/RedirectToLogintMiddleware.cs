using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Restaurants_Webpage.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RedirectToLogintMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _headerName = "Token-expired";

        public RedirectToLogintMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var tokenExpired = httpContext.Response.Headers[_headerName];

            if (!string.IsNullOrEmpty(tokenExpired) && bool.Parse(tokenExpired)) 
            {
                //redirect to login page
                httpContext.Response.Redirect("/");
            }

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RefreshJwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseRefreshJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RedirectToLogintMiddleware>();
        }
    }
}
