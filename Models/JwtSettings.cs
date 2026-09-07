using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Models
{
    public class JwtSettings
    {
        public const int MinimumSecretKeyLength = 32;

        /// <summary>Clave HMAC del token. Debe entregarse por configuración (variable Jwt__SecretKey), nunca en el código.</summary>
        [Required]
        [MinLength(MinimumSecretKeyLength)]
        public string SecretKey { get; set; } = null!;

        [Range(1, 720)]
        public int ExpirationHours { get; set; } = 12;
    }
}
