using EmployeeAPI.Models;
using EmployeeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        public AuthController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<User>> Post(User user)
        {
            if (user.Id is not null)
            {
                return BadRequest();
            }
            var listUser = await _mongoDBService.GetUsersAsync();
            if (listUser is null || listUser.Count==0)
            {
                await _mongoDBService.CreateUserAsync(new User { username = "admin", password = "admin" });

            }
            var userFind = await _mongoDBService.GetUserAsync(user.username);
            if (userFind is null)
            {
                return BadRequest();
            }
            if (userFind.password!= user.password)
            {
                return BadRequest();
            }
            if (userFind.token is null)
            {
                //Gernrar token Jwt con una key

                //guardar el tocken en la base de datos
                userFind = Tools.generateSecurityTokenDescriptor("00000000000000000000000000000000", userFind);
                await _mongoDBService.UpdateUserAsync(userFind.Id, userFind);
            }
            else
            {
                //valido que el token no haya expirado
                bool isTokenValid = Tools.IsTokenValid(userFind.token, "00000000000000000000000000000000");

                if (!isTokenValid)
                {
                    // El token ha expirado o es inválido
                    userFind = Tools.generateSecurityTokenDescriptor("00000000000000000000000000000000", userFind);
                    await _mongoDBService.UpdateUserAsync(userFind.Id, userFind);
                }
            }

            return userFind;
        }

        
    }
}
