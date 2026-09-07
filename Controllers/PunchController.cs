using EmployeeAPI.Models;
using EmployeeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PunchController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public PunchController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // GET: api/<PunchController>
        [HttpGet]
        public async Task<ActionResult<List<Punch>>> Get(
            [FromQuery] string? employeeId,
            [FromQuery] string? deviceId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            if (from.HasValue && to.HasValue && from > to)
            {
                return BadRequest("El parámetro 'from' no puede ser posterior a 'to'.");
            }

            return await _mongoDBService.GetPunchesAsync(employeeId, deviceId, from, to);
        }

        // GET api/<PunchController>/5
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Punch>> Get(string id)
        {
            var punch = await _mongoDBService.GetPunchAsync(id);

            if (punch is null)
            {
                return NotFound();
            }

            return punch;
        }

        [HttpGet("types")]
        public async Task<List<PunchType>> GetPunchTypes() =>
            await _mongoDBService.GetPunchTypesAsync();

        [HttpPost("types")]
        public async Task<IActionResult> PostPunchType(PunchType newPunchType)
        {
            if (newPunchType.Id is not null)
            {
                return BadRequest("El identificador es asignado por el servidor.");
            }

            newPunchType.Code = newPunchType.Code.Trim().ToUpperInvariant();

            var existing = await _mongoDBService.GetPunchTypeByCodeAsync(newPunchType.Code);
            if (existing is not null)
            {
                return Conflict($"A punch type with the code '{newPunchType.Code}' already exists.");
            }

            await _mongoDBService.CreatePunchTypeAsync(newPunchType);

            return CreatedAtAction(nameof(GetPunchTypes), newPunchType);
        }

        /// <summary>Registra una marca enviada por un dispositivo. El empleado se resuelve por Employee_Id, DNI o PIN.</summary>
        // POST api/<PunchController>
        [HttpPost]
        public async Task<IActionResult> Post(Punch newPunch)
        {
            if (newPunch.Id is not null)
            {
                return BadRequest("El identificador es asignado por el servidor.");
            }

            var device = await _mongoDBService.GetDeviceAsync(newPunch.Device_Id);
            if (device is null)
            {
                return NotFound($"Device '{newPunch.Device_Id}' was not found.");
            }

            newPunch.PunchType = newPunch.PunchType.Trim().ToUpperInvariant();
            var punchType = await _mongoDBService.GetPunchTypeByCodeAsync(newPunch.PunchType);
            if (punchType is null)
            {
                return BadRequest($"The punch type '{newPunch.PunchType}' is not registered.");
            }
            newPunch.PunchType_Id = punchType.Id;

            var employee = await ResolveEmployeeAsync(newPunch);
            if (employee is null)
            {
                return BadRequest("No fue posible identificar al empleado con los datos entregados (Employee_Id, Dni o Pin).");
            }

            newPunch.Employee_Id = employee.Id;
            newPunch.Dni ??= employee.Dni;
            newPunch.Timezone ??= device.Timezone;
            newPunch.Punch_Dtm = newPunch.Punch_Dtm == default
                ? DateTime.UtcNow
                : newPunch.Punch_Dtm.ToUniversalTime();

            if (newPunch.Punch_Dtm > DateTime.UtcNow.AddMinutes(5))
            {
                return BadRequest("La fecha de la marca no puede estar en el futuro.");
            }

            var duplicated = await _mongoDBService.GetDuplicatePunchAsync(employee.Id!, device.Id!, newPunch.Punch_Dtm);
            if (duplicated is not null)
            {
                return Conflict("La marca ya fue registrada para este empleado, dispositivo y fecha.");
            }

            newPunch.Status = PunchStatus.Valid;

            await _mongoDBService.CreatePunchAsync(newPunch);

            return CreatedAtAction(nameof(Get), new { id = newPunch.Id }, newPunch);
        }

        private async Task<Employee?> ResolveEmployeeAsync(Punch punch)
        {
            if (!string.IsNullOrEmpty(punch.Employee_Id))
            {
                return await _mongoDBService.GetAsync(punch.Employee_Id);
            }

            if (!string.IsNullOrEmpty(punch.Dni))
            {
                return await _mongoDBService.GetByDniAsync(punch.Dni);
            }

            if (!string.IsNullOrEmpty(punch.Pin))
            {
                var enrollment = await _mongoDBService.GetEnrollmentByPinAsync(punch.Pin, punch.Device_Id);
                if (enrollment is not null)
                {
                    return await _mongoDBService.GetAsync(enrollment.Employee_Id);
                }
            }

            return null;
        }
    }
}
