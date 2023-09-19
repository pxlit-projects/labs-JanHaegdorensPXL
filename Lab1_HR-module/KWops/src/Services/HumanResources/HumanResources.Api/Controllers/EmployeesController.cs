using HumanResources.AppLogic;
using HumanResources.Domain;
using Microsoft.AspNetCore.Mvc;

namespace HumanResources.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // GET /api/employees/{number}
        [HttpGet("{number}")]
        public async Task<IActionResult> GetEmployeeByNumber(string number)
        {
            var employee = await _employeeRepository.GetByNumberAsync(number);

            if (employee == null)
            {
                return NotFound(); 
            }

            return Ok(employee);
        }

        // POST /api/employees
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest(); // Invalid input
            }

            await _employeeRepository.AddAsync(employee);

            return Ok();
        }
    }
}
