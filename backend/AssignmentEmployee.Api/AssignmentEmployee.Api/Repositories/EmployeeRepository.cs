using AssignmentEmployee.Api.Data;
using AssignmentEmployee.Api.Models;
using Microsoft.EntityFrameworkCore;


namespace AssignmentEmployee.Api.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _db;
        public EmployeeRepository(ApplicationDbContext db) { _db = db; }


        public async Task<Employee> CreateAsync(Employee employee)
        {
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return employee;
        }


        public async Task DeleteAsync(int id)
        {
            var e = await _db.Employees.FindAsync(id);
            if (e == null) return;
            _db.Employees.Remove(e);
            await _db.SaveChangesAsync();
        }


        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _db.Employees.Include(x => x.Department).ToListAsync();
        }


        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _db.Employees.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task UpdateAsync(Employee employee)
        {
            _db.Employees.Update(employee);
            await _db.SaveChangesAsync();
        }
    }
}