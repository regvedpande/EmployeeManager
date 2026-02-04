using Microsoft.EntityFrameworkCore;
using AssignmentEmployee.Api.Models; // 👈 Ensures we can use the 'User' class

namespace AssignmentEmployee.Api.Data
{
    // 👇 Change IdentityDbContext to DbContext (Simpler for now)
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        // 👇 THIS WAS MISSING! This connects your User model to the database.
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Data
            builder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "HR" },
                new Department { Id = 2, Name = "Engineering" },
                new Department { Id = 3, Name = "Sales" }
            );
        }
    }
}