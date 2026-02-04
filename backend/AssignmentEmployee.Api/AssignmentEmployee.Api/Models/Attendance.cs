using System.ComponentModel.DataAnnotations;


namespace AssignmentEmployee.Api.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime Date { get; set; }
        public bool Present { get; set; }
    }
}