using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MS.Extensions.Security;

namespace MS.Areas.Security.Pages
{
    [AllowAnonymous]
    public class LoginWithRecoveryCodeModel : ModelBase
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [BindProperty]
            [Required(ErrorMessage = "{0}²»ÄÜÎª¿Õ£¡")]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; }
        }
        
        private readonly IUserManager _userManager;

        public LoginWithRecoveryCodeModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _userManager.SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

            var result = await _userManager.SignInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
            
            if (result.Succeeded)
            {
                Logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.UserId);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            if (result.IsLockedOut)
            {
                Logger.LogWarning("User with ID '{UserId}' account locked out.", user.UserId);
                return RedirectToPage("./Lockout");
            }

            Logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.UserId);
            ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
            return Page();
        }
    }
}
