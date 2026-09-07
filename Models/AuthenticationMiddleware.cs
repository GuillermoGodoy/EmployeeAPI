using EmployeeAPI.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace EmployeeAPI.Models
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _jwtSettings;

        public AuthenticationMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == "OPTIONS")
            {
                SetCorsHeaders(context);
                context.Response.StatusCode = 200;
                //await _next.Invoke(context);
                return;
            }
            if ((context.Request.Path.Value?.Equals("/health", StringComparison.OrdinalIgnoreCase) == true) && (context.Request.Method == "GET"))
            {
                SetCorsHeaders(context);
                await _next.Invoke(context);
                return;
            }
            if ((context.Request.Path.Value?.Equals("/api/auth/login", StringComparison.OrdinalIgnoreCase) == true) && (context.Request.Method == "POST"))
            {
                SetCorsHeaders(context);
                await _next.Invoke(context);
                return;
            }
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                SetCorsHeaders(context);
                await _next.Invoke(context);
                return;
            }
            //Validar bearer token
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
                //validar si el token es valido, si es valido continuar con el request, sino retornar 401
                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);
                }
                catch (Exception)
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = 401;
                return;
            }
            SetCorsHeaders(context);
            await _next.Invoke(context);
        }

        private static void SetCorsHeaders(HttpContext context)
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = context.Request.Headers["Origin"];
            context.Response.Headers["Access-Control-Allow-Headers"] = "Origin, X-Requested-With, Content-Type, Accept, Authorization, Access-Control-Allow-Origin";
            context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS, PATCH";
            context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
        }
    }
}
