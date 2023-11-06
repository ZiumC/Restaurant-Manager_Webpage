using System.IdentityModel.Tokens.Jwt;

namespace Restaurants_Webpage.Utils
{
    public enum JwtFields { NAME, ROLE, EMP_ID, CLIENT_ID }
    public class HttpJwtUtility
    {
        private readonly string? _jwtCookie;
        private readonly string _jwtCookieNameField;
        private readonly string _jwtCookieRoleField;
        private readonly string _jwtCookieEmpIdField;
        private readonly string _jwtCookieClientIdField;
        private readonly IConfiguration _config;

        public HttpJwtUtility(IConfiguration config, HttpContext httpContext)
        {
            _config = config;

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

                _jwtCookie = httpContext.Request.Cookies[jwtCookieName];

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

        public string? GetJwtCookieValue(JwtFields jwtCookieField)
        {
            if (_jwtCookie != null)
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

                var tokenContent = new JwtSecurityTokenHandler().ReadToken(_jwtCookie) as JwtSecurityToken;

                return tokenContent?.Claims.First(claim => claim.Type == fieldName).Value;
            }

            return null;
        }
    }
}
