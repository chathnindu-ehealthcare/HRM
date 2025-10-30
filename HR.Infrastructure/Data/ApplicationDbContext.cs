using HR.Domain.Identity;
using HR.Domain.Employees;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Designation> Designations => Set<Designation>();
        public DbSet<EmployeeCategory> EmployeeCategories => Set<EmployeeCategory>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<EmployeeHistory> EmployeeHistories => Set<EmployeeHistory>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Unique indexes (adjust to your policy)
            b.Entity<Employee>().HasIndex(e => e.NIC).IsUnique(false);   // set true if NIC must be unique
            b.Entity<Employee>().HasIndex(e => e.Email).IsUnique(false); // set true if Email must be unique

            b.Entity<Department>().HasIndex(x => x.Name).IsUnique();
            b.Entity<Designation>().HasIndex(x => x.Name).IsUnique();
            b.Entity<EmployeeCategory>().HasIndex(x => x.Name).IsUnique();

            // Length constraints (optional but good)
            b.Entity<Employee>().Property(x => x.FullName).HasMaxLength(120).IsRequired();
            b.Entity<Employee>().Property(x => x.EpfNo).HasMaxLength(20);
            b.Entity<Employee>().Property(x => x.EtfNo).HasMaxLength(20);
        }
    }
}
