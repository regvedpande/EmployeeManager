using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssignmentEmployee.Api.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        // 🔥 IMPORTANT
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
