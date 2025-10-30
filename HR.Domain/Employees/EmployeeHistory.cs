namespace HR.Domain.Employees
{
    public class EmployeeHistory
    {
        public long Id { get; set; }
        public int EmployeeId { get; set; }
        public string Action { get; set; } = default!;      // Created, Updated, Deleted
        public string? Field { get; set; }                  // Optional: which field changed
        public string? FromValue { get; set; }
        public string? ToValue { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? ByUser { get; set; }
        public Employee? Employee { get; set; }
    }
}
