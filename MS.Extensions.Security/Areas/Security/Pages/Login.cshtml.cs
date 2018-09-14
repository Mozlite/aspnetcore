using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Activities;
using MS.Extensions.Security;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MS.Areas.Security.Pages
{
    /// <summary>
    /// 登陆模型。
    /// </summary>
    [AllowAnonymous]
    public class LoginModel : ModelBase
    {
        private readonly IUserManager _userManager;

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "用户名不能为空!")]
            public string Name { get; set; }

            [Required(ErrorMessage = "密码不能为空!")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "验证码不能为空!")]
            public string Code { get; set; }

            public bool RememberMe { get; set; }
        }

        public LoginModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _userManager.SignInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                if (!IsValidateCode("login", Input.Code))
                {
                    ModelState.AddModelError("Input.Code", "验证码不正确！");
                    return Page();
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _userManager.PasswordSignInAsync(Input.Name, Input.Password, Input.RememberMe,
                    current =>
                    {
                        Logger.Info("登陆了应用程序。");
                        return Task.CompletedTask;
                    });
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    Logger.Info("账号被锁定。");
                    return RedirectToPage("./Lockout");
                }

                ModelState.AddModelError(string.Empty, "用户名或密码错误。");
                return Page();
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
