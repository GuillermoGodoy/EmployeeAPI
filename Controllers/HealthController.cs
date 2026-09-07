using EmployeeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public HealthController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        /// <summary>Verifica que la API responde y que la base de datos está accesible.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Get()
        {
            var checkedAtUtc = DateTime.UtcNow;

            try
            {
                await _mongoDBService.PingAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    status = "Unhealthy",
                    database = "Unreachable",
                    error = ex.GetType().Name,
                    checkedAtUtc
                });
            }

            return Ok(new { status = "Healthy", database = "Reachable", checkedAtUtc });
        }
    }
}
