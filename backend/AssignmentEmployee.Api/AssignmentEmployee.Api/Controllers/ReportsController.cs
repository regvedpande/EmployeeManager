using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssignmentEmployee.Api.Data;
using System.Text;

namespace AssignmentEmployee.Api.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/reports/employees
        [HttpGet("employees")]
        public async Task<IActionResult> ExportEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Id,FullName,Email,Department,Salary");

            foreach (var e in employees)
            {
                csv.AppendLine($"{e.Id},{e.FullName},{e.Email},{e.Department?.Name},{e.Salary}");
            }

            return File(
                Encoding.UTF8.GetBytes(csv.ToString()),
                "text/csv",
                "employees-report.csv"
            );
        }
    }
}
