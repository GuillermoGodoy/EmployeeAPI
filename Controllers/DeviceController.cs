using EmployeeAPI.Models;
using EmployeeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public DeviceController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // GET: api/<DeviceController>
        [HttpGet]
        public async Task<List<Device>> Get() =>
            await _mongoDBService.GetDevicesAsync();

        // GET api/<DeviceController>/5
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Device>> Get(string id)
        {
            var device = await _mongoDBService.GetDeviceAsync(id);

            if (device is null)
            {
                return NotFound();
            }

            return device;
        }

        // POST api/<DeviceController>
        [HttpPost]
        public async Task<IActionResult> Post(Device newDevice)
        {
            if (newDevice.Id is not null)
            {
                return BadRequest("El identificador es asignado por el servidor.");
            }

            var existingDevice = await _mongoDBService.GetDeviceByNameAsync(newDevice.Name);
            if (existingDevice is not null)
            {
                return Conflict($"A device with the name '{newDevice.Name}' already exists.");
            }

            await _mongoDBService.CreateDeviceAsync(newDevice);

            return CreatedAtAction(nameof(Get), new { id = newDevice.Id }, newDevice);
        }

        // PUT api/<DeviceController>/5
        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult<Device>> Update(string id, Device updatedDevice)
        {
            var device = await _mongoDBService.GetDeviceAsync(id);

            if (device is null)
            {
                return NotFound();
            }

            updatedDevice.Id = device.Id;

            await _mongoDBService.UpdateDeviceAsync(id, updatedDevice);

            return updatedDevice;
        }

        // DELETE api/<DeviceController>/5
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var device = await _mongoDBService.GetDeviceAsync(id);

            if (device is null)
            {
                return NotFound();
            }

            await _mongoDBService.RemoveDeviceAsync(id);

            return NoContent();
        }
    }
}
