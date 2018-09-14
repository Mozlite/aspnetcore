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
    public class LoginWith2faModel : ModelBase
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
        }
        
        private readonly IUserManager _userManager;

        public LoginWith2faModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _userManager.SignInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException("未找到二次验证登陆的用户。");
            }

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _userManager.SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _userManager.SignInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);
            
            if (result.Succeeded)
            {
                Logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.UserId);
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                Logger.LogWarning("User with ID '{UserId}' account locked out.", user.UserId);
                return RedirectToPage("./Lockout");
            }

            Logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.UserId);
            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            return Page();
        }
    }
}
