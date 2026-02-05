using AssignmentEmployee.Api.Data;
using AssignmentEmployee.Api.DTOs;
using AssignmentEmployee.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public EmployeesController(ApplicationDbContext db)
    {
        _db = db;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst("id")!.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var userId = GetUserId();

        var employees = await _db.Employees
            .Include(e => e.Department)
            .Where(e => e.UserId == userId)
            .Select(e => new
            {
                e.Id,
                e.FullName,
                e.Email,
                e.Salary,
                department = e.Department == null ? null : new { name = e.Department.Name }
            })
            .ToListAsync();

        return Ok(employees);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var userId = GetUserId();

        var employee = await _db.Employees
            .Where(e => e.Id == id && e.UserId == userId)
            .Select(e => new
            {
                e.Id,
                e.FullName,
                e.Email,
                e.Salary,
                e.DepartmentId
            })
            .FirstOrDefaultAsync();

        if (employee == null) return NotFound();

        return Ok(employee);
    }


    [HttpPost]
    public async Task<IActionResult> CreateEmployee(EmployeeDto dto)
    {
        var userId = GetUserId();

        var employee = new Employee
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Salary = dto.Salary,
            DepartmentId = dto.DepartmentId,
            UserId = userId // 🔥 THIS FIXES 500 ERROR
        };

        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, EmployeeDto dto)
    {
        var userId = GetUserId();

        var employee = await _db.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (employee == null) return NotFound();

        employee.FullName = dto.FullName;
        employee.Email = dto.Email;
        employee.Salary = dto.Salary;
        employee.DepartmentId = dto.DepartmentId;

        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var userId = GetUserId();

        var employee = await _db.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (employee == null) return NotFound();

        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync();

        return Ok();
    }
}
