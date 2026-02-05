using AssignmentEmployee.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentEmployee.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public DepartmentsController(ApplicationDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> Get()
            => Ok(await _db.Departments.OrderBy(d => d.Name).ToListAsync());
    }
}
