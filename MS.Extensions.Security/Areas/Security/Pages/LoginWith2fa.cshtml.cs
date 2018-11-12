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
            [Required(ErrorMessage = "{0}不能为空！")]
            [StringLength(7, ErrorMessage = "{0}的长度必须在{2}和{1}之间。", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "验证码")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "记住这个设备信息")]
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
