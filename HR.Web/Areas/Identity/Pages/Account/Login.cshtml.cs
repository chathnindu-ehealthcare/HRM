#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using HR.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace HR.Web.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required, Display(Name = "Username or Email")]
            public string UserNameOrEmail { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (!ModelState.IsValid) return Page();

            // Let Identity accept either username or email for convenience
            var userName = Input.UserNameOrEmail;
            if (Input.UserNameOrEmail.Contains("@"))
                userName = Input.UserNameOrEmail; // email works as username in default Identity

            var result = await _signInManager.PasswordSignInAsync(userName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded) return LocalRedirect(returnUrl);

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
