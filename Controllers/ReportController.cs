using EmployeeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        // POST api/Report/generate -> inicia el job y retorna el id de ejecución
        [HttpPost("generate")]
        public IActionResult Generate()
        {
            var executionId = _reportService.StartReport();

            return AcceptedAtAction(nameof(GetStatus), new { id = executionId }, new { executionId });
        }

        // GET api/Report/{id}/status -> consulta el estado del job (Processing | Completed)
        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetStatus(string id)
        {
            var job = await _reportService.GetStatusAsync(id);

            if (job is null)
            {
                return NotFound();
            }

            return Ok(job);
        }
    }
}
