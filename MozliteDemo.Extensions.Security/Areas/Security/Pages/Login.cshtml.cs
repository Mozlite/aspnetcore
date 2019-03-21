using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Settings;
using MozliteDemo.Extensions.Security.Areas.Security.Models;
using MozliteDemo.Extensions.Security.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages
{
    /// <summary>
    /// 登陆模型。
    /// </summary>
    [AllowAnonymous]
    public class LoginModel : ModelBase
    {
        private readonly IUserManager _userManager;
        private readonly ISettingsManager _settingsManager;

        [BindProperty]
        public SigninUser Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public LoginModel(IUserManager userManager, ISettingsManager settingsManager)
        {
            _userManager = userManager;
            _settingsManager = settingsManager;
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            var settings = await _settingsManager.GetSettingsAsync<SecuritySettings>();
            returnUrl = returnUrl ?? Url.GetDirection(settings.LoginDirection);

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _userManager.SignInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
#if !DEBUG
                if (!IsValidateCode("login", Input.Code))
                {
                    ModelState.AddModelError("Input.Code", "验证码不正确！");
                    return Page();
                }
#endif
                var settings = await _settingsManager.GetSettingsAsync<SecuritySettings>();
                returnUrl = returnUrl ?? Url.GetDirection(settings.LoginDirection);
                Input.UserName = Input.UserName.Trim();
                Input.Password = Input.Password.Trim();

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _userManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, async user => await EventLogger.LogAsync(user.UserId, Resources.EventType, "成功登入系统。"));
                if (result.Succeeded)
                {
                    Response.Cookies.Delete("login");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    Logger.LogWarning($"账号：{Input.UserName}被锁定。");
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
