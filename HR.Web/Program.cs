using HR.Domain.Identity;
using HR.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=.;Database=HRM;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(cs));

// We want roles, so use AddIdentity (not AddDefaultIdentity)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o =>
{
    o.User.RequireUniqueEmail = true;
    o.Password.RequiredLength = 8;
    o.Password.RequireDigit = true;
    o.Password.RequireLowercase = true;
    o.Password.RequireUppercase = false;
    o.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
// Optional: include the built-in default UI for the pages you DON'T override
.AddDefaultUI(); // adds RCL UI for Identity area
// (You can scaffold or hand-write only the pages you want to customize.) :contentReference[oaicite:1]{index=1}

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Identity UI uses Razor Pages

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // expose /Identity/... endpoints

await HR.Web.Seed.IdentitySeeder.SeedAsync(app.Services); // step 7 below
app.Run();
