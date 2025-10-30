using HR.Domain.Employees;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Data.Seed
{
    public static class InitialSeed
    {
        public static async Task RunAsync(ApplicationDbContext db)
        {
            if (!await db.Departments.AnyAsync())
            {
                db.Departments.AddRange(
                    new Department { Name = "HR" },
                    new Department { Name = "Finance" },
                    new Department { Name = "IT" },
                    new Department { Name = "Operations" }
                );
            }
            if (!await db.Designations.AnyAsync())
            {
                db.Designations.AddRange(
                    new Designation { Name = "Executive" },
                    new Designation { Name = "Assistant" },
                    new Designation { Name = "Manager" },
                    new Designation { Name = "Officer" }
                );
            }
            if (!await db.EmployeeCategories.AnyAsync())
            {
                db.EmployeeCategories.AddRange(
                    new EmployeeCategory { Name = "Permanent" },
                    new EmployeeCategory { Name = "Part-Time" },
                    new EmployeeCategory { Name = "Contract" },
                    new EmployeeCategory { Name = "Consultant" }
                );
            }
            await db.SaveChangesAsync();
        }
    }
}
