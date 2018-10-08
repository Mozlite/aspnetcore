using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MS.Extensions.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MS.Areas.Security.Pages.Account
{
    public class ExternalLoginsModel : ModelBase
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        private readonly IUserManager _userManager;

        public ExternalLoginsModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            CurrentLogins = await _userManager.GetLoginsAsync(user);
            OtherLogins = (await _userManager.SignInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();

            ShowRemoveButton = user.PasswordHash != null || CurrentLogins.Count > 1;
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"{user.UserName}移除社会化登陆失败！");
            }

            await _userManager.SignInManager.RefreshSignInAsync(user);
            return RedirectToSuccessPage("你已经成功移除了登陆。");
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Page("./ExternalLogins", "LinkLoginCallback");
            var properties = _userManager.SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, UserId.ToString());
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            var info = await _userManager.SignInManager.GetExternalLoginInfoAsync(user.UserId.ToString());
            if (info == null)
            {
                throw new InvalidOperationException("载入社会化登陆失败！");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("添加社会化登陆失败！");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return RedirectToSuccessPage($"已经成功绑定了{info.ProviderDisplayName}登陆方式。");
        }
    }
}
