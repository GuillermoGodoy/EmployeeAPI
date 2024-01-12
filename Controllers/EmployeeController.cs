using EmployeeAPI.Models;
using EmployeeAPI.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        public EmployeeController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }
        // GET: api/<EmployeeController>
        [HttpGet]
        public async Task<List<Employee>> Get() =>
        await _mongoDBService.GetAsync();

        // GET api/<EmployeeController>/5
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Employee>> Get(string id)
        {
            var Employee = await _mongoDBService.GetAsync(id);

            if (Employee is null)
            {
                return NotFound();
            }

            return Employee;
        }

        // POST api/<EmployeeController>
        [HttpPost]
        public async Task<IActionResult> Post(Employee newEmployee)
        {
            if (newEmployee.Id is not null)
            {
                return BadRequest();
            }
            await _mongoDBService.CreateAsync(newEmployee);

            return CreatedAtAction(nameof(Get), new { id = newEmployee.Id }, newEmployee);
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult<Employee>> Update(string id, Employee updatedEmployee)
        {
            var Emp = await _mongoDBService.GetAsync(id);

            if (Emp is null)
            {
                return NotFound();
            }

            updatedEmployee.Id = Emp.Id;

            await _mongoDBService.UpdateAsync(id, updatedEmployee);

            return updatedEmployee;
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _mongoDBService.GetAsync(id);

            if (book is null)
            {
                return NotFound();
            }

            await _mongoDBService.RemoveAsync(id);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(string id, PatchEmployee employee)
        {
            // Obtener los campos del body
            employee.Id = id;
            var updatedEmployee = employee.ToDictionary();

            // Actualizar el objeto
            var employeeFromDb = await _mongoDBService.GetAsync(id);

            if (employeeFromDb == null)
            {
                return NotFound();
            }
            
            foreach (var key in updatedEmployee.Keys)
            {
                if (typeof(Employee).GetProperty(key) != null)
                {
                    if (updatedEmployee[key] != null)
                    {
                        typeof(Employee).GetProperty(key).SetValue(employeeFromDb, updatedEmployee[key]);
                    }
                }
            }

            await _mongoDBService.UpdateAsync(id, employeeFromDb);

            return employeeFromDb;
        }
    }
}
