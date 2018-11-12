using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MS.Extensions.Security;

namespace MS.Areas.Security.Pages
{
    /// <summary>
    /// 外部登陆模型。
    /// </summary>
    [AllowAnonymous]
    public class ExternalLoginModel : ModelBase
    {
        private readonly IUserManager _userManager;

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            /// <summary>
            /// 用户名称。
            /// </summary>
            [Required(ErrorMessage = "{0}不能为空！")]
            public string UserName { get; set; }

            /// <summary>
            /// 登陆名称。
            /// </summary>
            [Required(ErrorMessage = "{0}不能为空！")]
            public string LoginName { get; set; }

            [EmailAddress]
            public string Email { get; set; }
        }
        
        public ExternalLoginModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _userManager.SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"社会化登陆提供者错误：{remoteError}。";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _userManager.SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "载入社会化登陆信息错误！";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _userManager.SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                Logger.LogInformation("{Name} 通过 {LoginProvider} 进行登陆。", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            // If the user does not have an account, then ask the user to create an account.
            ReturnUrl = returnUrl;
            ProviderDisplayName = info.ProviderDisplayName;
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                Input = new InputModel
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                };
            }
            return Page();
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _userManager.SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "确认社会化登陆失败！";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var user = new User();
                user.UserName = Input.UserName;
                user.NormalizedUserName = _userManager.NormalizeKey(Input.LoginName);
                user.Email = Input.Email;
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _userManager.SignInManager.SignInAsync(user, isPersistent: false);
                        Logger.LogInformation("用户通过 {Name} 新注册了一个用户。", info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
