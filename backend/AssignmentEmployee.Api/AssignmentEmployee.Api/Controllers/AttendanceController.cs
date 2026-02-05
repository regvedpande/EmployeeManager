using AssignmentEmployee.Api.Data;
using AssignmentEmployee.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentEmployee.Api.Controllers
{
    public class AttendanceDto
    {
        public int EmployeeId { get; set; }
        public bool Present { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST api/attendance
        [HttpPost]
        public async Task<IActionResult> MarkAttendance([FromBody] AttendanceDto dto)
        {
            var today = DateTime.UtcNow.Date;

            var existing = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == dto.EmployeeId && a.Date == today);

            if (existing != null)
            {
                existing.Present = dto.Present;
                await _context.SaveChangesAsync();
                return Ok(existing);
            }

            var attendance = new Attendance
            {
                EmployeeId = dto.EmployeeId,
                Date = today,
                Present = dto.Present
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok(attendance);
        }

        // GET: api/attendance/{employeeId}
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetAttendance(int employeeId)
        {
            var records = await _context.Attendances
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return Ok(records);
        }
    }
}
