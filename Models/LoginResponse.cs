namespace EmployeeAPI.Models
{
    /// <summary>Respuesta del login. Nunca debe incluir la contraseña ni el documento completo del usuario.</summary>
    public class LoginResponse
    {
        public string Username { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }
    }
}
