using System.ComponentModel.DataAnnotations;

namespace HR.Domain.Employees
{
    public class Employee
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string FullName { get; set; } = default!;

        [StringLength(12)]
        public string? NIC { get; set; }  // Make this unique if used in your org

        [StringLength(200)]
        public string? Address { get; set; }

        [EmailAddress, StringLength(120)]
        public string? Email { get; set; }

        [Phone, StringLength(20)]
        public string? Phone { get; set; }

        // FKs
        [Required] public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        [Required] public int DesignationId { get; set; }
        public Designation? Designation { get; set; }

        [Required] public int EmployeeCategoryId { get; set; }
        public EmployeeCategory? EmployeeCategory { get; set; }

        [StringLength(20)]
        public string? EpfNo { get; set; }

        [StringLength(20)]
        public string? EtfNo { get; set; }

        public DateTime DateOfJoining { get; set; } = DateTime.UtcNow.Date;
        public bool IsActive { get; set; } = true;
    }
}
