using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Restaurants_Webpage.Utils
{
    public enum JwtFields { NAME, ROLE, EMP_ID, CLIENT_ID }
    public class HttpJwtUtility
    {
        private readonly string _jwtCookieName;
        private readonly string _jwtCookieNameField;
        private readonly string _jwtCookieRoleField;
        private readonly string _jwtCookieEmpIdField;
        private readonly string _jwtCookieClientIdField;
        private readonly HttpContext _httpContext;
        private readonly IConfiguration _config;

        public HttpJwtUtility(IConfiguration config, HttpContext httpContext)
        {
            _config = config;
            _httpContext = httpContext;

            string jwtCookieName = _config["ApplicationSettings:JwtSettings:CookieSettings:CookieName"];

            string jwtCookieNameField = _config["ApplicationSettings:UserSettings:CookieSettings:Common:FieldName"];
            string jwtCookieRoleField = _config["ApplicationSettings:UserSettings:CookieSettings:Common:RoleName"];
            string jwtCookieClientIdField = _config["ApplicationSettings:UserSettings:CookieSettings:Client:IdName"];
            string jwtCookieEmpIdField = _config["ApplicationSettings:UserSettings:CookieSettings:Supervisor:IdName"];

            try
            {
                if (string.IsNullOrEmpty(jwtCookieName))
                {
                    throw new Exception("Jwt cookie namne can't be empty");
                }

                if (string.IsNullOrEmpty(jwtCookieNameField))
                {
                    throw new Exception("Jwt cookie name field can't be empty");
                }

                if (string.IsNullOrEmpty(jwtCookieRoleField))
                {
                    throw new Exception("Jwt cookie role name can't be empty");
                }

                if (string.IsNullOrEmpty(jwtCookieClientIdField))
                {
                    throw new Exception("Jwt cookie ClientId name can't be empty");
                }

                if (string.IsNullOrEmpty(jwtCookieEmpIdField))
                {
                    throw new Exception("Jwt cookie EmpId name can't be empty");
                }

                _jwtCookieName = jwtCookieName;
                _jwtCookieNameField = jwtCookieNameField;
                _jwtCookieRoleField = jwtCookieRoleField;
                _jwtCookieClientIdField = jwtCookieClientIdField;
                _jwtCookieEmpIdField = jwtCookieEmpIdField;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string? GetJwtRequestCookie()
        {
            return _httpContext.Request.Cookies[_jwtCookieName];
        }

        public string? GetJwtRequestCookieValue(string fieldName, string? jwtCookie)
        {
            var tokenContent = new JwtSecurityTokenHandler().ReadToken(jwtCookie) as JwtSecurityToken;

            return tokenContent?.Claims.First(claim => claim.Type == fieldName).Value;
        }

        public string? GetJwtRequestCookieValue(JwtFields jwtCookieField, string? jwtCookie)
        {
            string fieldName = "";

            switch (jwtCookieField)
            {
                case JwtFields.NAME:
                    fieldName = _jwtCookieNameField;
                    break;

                case JwtFields.ROLE:
                    fieldName = _jwtCookieRoleField;
                    break;

                case JwtFields.EMP_ID:
                    fieldName = _jwtCookieEmpIdField;
                    break;

                case JwtFields.CLIENT_ID:
                    fieldName = _jwtCookieClientIdField;
                    break;

                default:
                    return null;
            }

            var tokenContent = new JwtSecurityTokenHandler().ReadToken(jwtCookie) as JwtSecurityToken;

            return tokenContent?.Claims.First(claim => claim.Type == fieldName).Value;

        }
    }
}
