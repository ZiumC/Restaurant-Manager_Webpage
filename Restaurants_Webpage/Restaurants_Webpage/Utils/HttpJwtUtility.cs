using System.IdentityModel.Tokens.Jwt;

namespace Restaurants_Webpage.Utils
{
    public class HttpJwtUtility
    {
        private readonly string? _jwtCookie;
        private readonly IConfiguration _config;

        public HttpJwtUtility(IConfiguration config, HttpContext httpContext)
        {
            _config = config;

            string jwtCookieName = _config["ApplicationSettings:JwtSettings:CookieSettings:CookieName"];

            try
            {
                if (string.IsNullOrEmpty(jwtCookieName))
                {
                    throw new Exception("Jwt cookie namne can't be empty");
                }

                _jwtCookie = httpContext.Request.Cookies[jwtCookieName];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string? GetJwtCookie() 
        {
            return _jwtCookie;
        }

        public string? GetJwtCookieValue(string fieldName)
        {
            if (_jwtCookie != null)
            {
                var tokenContent = new JwtSecurityTokenHandler().ReadToken(_jwtCookie) as JwtSecurityToken;
                return tokenContent?.Claims.First(claim => claim.Type == fieldName).Value;
            }

            return null;
        }
    }
}
