using Microsoft.AspNetCore.Mvc;
using MS.Extensions.Security;
using System;
using System.Threading.Tasks;

namespace MS.Areas.Security.Pages.Account
{
    /// <summary>
    /// 禁用二次登陆验证。
    /// </summary>
    public class Disable2faModel : ModelBase
    {
        private readonly IUserManager _userManager;

        public Disable2faModel(IUserManager userManager)
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

            if (!user.TwoFactorEnabled)
            {
                throw new InvalidOperationException($"不能禁用'{user.UserName}'二次登陆验证，因为该账户没有开启二次登陆验证。");
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
            if (!await _userManager.UpdateAsync(user.UserId, new { user.TwoFactorEnabled }))
            {
                throw new InvalidOperationException($"禁用'{user.UserName}'二次登陆验证发生了错误！");
            }

            Log("禁用了二次登陆验证！");
            return RedirectToSuccessPage("二次登陆验证已经禁用。", "./TwoFactorAuthentication");
        }
    }
}