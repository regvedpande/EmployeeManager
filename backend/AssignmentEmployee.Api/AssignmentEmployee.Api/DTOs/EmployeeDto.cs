namespace AssignmentEmployee.Api.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int DepartmentId { get; set; }
        public decimal Salary { get; set; }
    }
}