namespace HR.Domain.Identity
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
