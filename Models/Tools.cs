using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeAPI.Models
{
    public class Tools
    {
        static public User generateSecurityTokenDescriptor(string secretKey, User userFind)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, userFind.username)
                }),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                   SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            userFind.token = tokenHandler.WriteToken(token);
            //agrego el timespan de expiracion
            userFind.tokenExpiration = tokenDescriptor.Expires.Value.TimeOfDay;
            return userFind;
        }
        static public bool IsTokenValid(string token, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Configurar la validación del token
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // No permitir margen de tiempo para expiración
                };

                // Validar el token
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                // Verificar si el token ha expirado
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var expirationDate = jwtSecurityToken.ValidTo;

                    // Comparar con la fecha y hora actual
                    if (expirationDate <= DateTime.UtcNow)
                    {
                        // Token válido y no ha expirado
                        return true;
                    }
                }

                // El token ha expirado
                return false;
            }
            catch (Exception)
            {
                // Error al validar el token
                return false;
            }
        }
    }
}
