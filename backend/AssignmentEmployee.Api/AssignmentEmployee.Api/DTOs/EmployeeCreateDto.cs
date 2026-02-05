namespace AssignmentEmployee.Api.DTOs
{
    public class EmployeeCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public int DepartmentId { get; set; }
    }

}
