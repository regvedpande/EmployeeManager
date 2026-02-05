using AssignmentEmployee.Api.Data;
using AssignmentEmployee.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentEmployee.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);

            var employees = await _context.Employees
                .Where(e => e.UserId == userId)
                .Include(e => e.Department)
                .ToListAsync();

            return Ok(employees);
        }


        // ✅ GET BY ID (FOR EDIT)
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            return employee;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeDto dto)
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);

            var employee = new Employee
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Salary = dto.Salary,
                DepartmentId = dto.DepartmentId,
                UserId = userId // 🔥 CRITICAL
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }


        // ✅ UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            employee.FullName = dto.FullName;
            employee.Email = dto.Email;
            employee.Salary = dto.Salary;
            employee.DepartmentId = dto.DepartmentId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    // 🔽 DTO (MATCHES FRONTEND EXACTLY)
    public class EmployeeDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public int DepartmentId { get; set; }
    }
}
