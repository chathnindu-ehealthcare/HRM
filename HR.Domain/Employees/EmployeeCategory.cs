namespace HR.Domain.Employees
{
    public class EmployeeCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!; // Permanent/Part-Time/Contract/Consultant
        public bool IsActive { get; set; } = true;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
