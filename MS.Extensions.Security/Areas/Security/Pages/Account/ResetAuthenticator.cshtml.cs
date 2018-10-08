using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MS.Extensions.Security;

namespace MS.Areas.Security.Pages.Account
{
    public class ResetAuthenticatorModel : ModelBase
    {
        private readonly IUserManager _userManager;

        public ResetAuthenticatorModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            user.TwoFactorEnabled = false;
            await _userManager.ResetAuthenticatorKeyAsync(user);
            Log("重置了验证密钥！");

            await _userManager.SignInManager.RefreshSignInAsync(user);
            return RedirectToSuccessPage("你已经重置了你的验证密钥，你需要重新使用设置验证密钥。", "./EnableAuthenticator");
        }
    }
}