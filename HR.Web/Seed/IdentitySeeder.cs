using HR.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace HR.Web.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "HR", "Manager", "Employee" };
            foreach (var r in roles)
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new IdentityRole(r));

            var adminEmail = "admin@hrm.local";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin@12345");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
