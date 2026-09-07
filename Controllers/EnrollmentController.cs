using EmployeeAPI.Models;
using EmployeeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public EnrollmentController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // GET: api/<EnrollmentController>
        [HttpGet]
        public async Task<List<Enrollment>> Get([FromQuery] string? employeeId, [FromQuery] string? deviceId) =>
            await _mongoDBService.GetEnrollmentsAsync(employeeId, deviceId);

        // GET api/<EnrollmentController>/5
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Enrollment>> Get(string id)
        {
            var enrollment = await _mongoDBService.GetEnrollmentAsync(id);

            if (enrollment is null)
            {
                return NotFound();
            }

            return enrollment;
        }

        // POST api/<EnrollmentController>
        [HttpPost]
        public async Task<IActionResult> Post(Enrollment newEnrollment)
        {
            if (newEnrollment.Id is not null)
            {
                return BadRequest("El identificador es asignado por el servidor.");
            }

            var employee = await _mongoDBService.GetAsync(newEnrollment.Employee_Id);
            if (employee is null)
            {
                return NotFound($"Employee '{newEnrollment.Employee_Id}' was not found.");
            }

            if (newEnrollment.Device_Id is not null)
            {
                var device = await _mongoDBService.GetDeviceAsync(newEnrollment.Device_Id);
                if (device is null)
                {
                    return NotFound($"Device '{newEnrollment.Device_Id}' was not found.");
                }
            }

            var existing = await _mongoDBService.GetEnrollmentByPinAsync(newEnrollment.Pin, newEnrollment.Device_Id);
            if (existing is not null)
            {
                return Conflict($"The pin '{newEnrollment.Pin}' is already in use.");
            }

            await _mongoDBService.CreateEnrollmentAsync(newEnrollment);

            return CreatedAtAction(nameof(Get), new { id = newEnrollment.Id }, newEnrollment);
        }

        // DELETE api/<EnrollmentController>/5
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var enrollment = await _mongoDBService.GetEnrollmentAsync(id);

            if (enrollment is null)
            {
                return NotFound();
            }

            await _mongoDBService.RemoveEnrollmentAsync(id);

            return NoContent();
        }
    }
}
