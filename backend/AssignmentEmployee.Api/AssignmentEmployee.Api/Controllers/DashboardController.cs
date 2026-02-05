using AssignmentEmployee.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentEmployee.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);

            var totalEmployees = await _context.Employees
                .CountAsync(e => e.UserId == userId);

            var totalSalary = await _context.Employees
                .Where(e => e.UserId == userId)
                .SumAsync(e => e.Salary);

            return Ok(new
            {
                totalEmployees,
                totalSalary
            });
        }
    }
}
