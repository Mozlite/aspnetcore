using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MS.Extensions.Security;

namespace MS.Areas.Security.Pages.Account
{
    public class GenerateRecoveryCodesModel : ModelBase
    {
        [TempData]
        public string[] RecoveryCodes { get; set; }
        
        private readonly IUserManager _userManager;

        public GenerateRecoveryCodesModel(IUserManager userManager)
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
            
            if (!user.TwoFactorEnabled)
            {
                throw new InvalidOperationException($"{user.UserName}未激活二次登陆验证，不能生成验证码。");
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
            
            if (!user.TwoFactorEnabled)
            {
                throw new InvalidOperationException($"{user.UserName}未激活二次登陆验证，不能生成验证码。");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            RecoveryCodes = recoveryCodes.ToArray();

            Log("生成10个二次登陆验证码。");
            return RedirectToSuccessPage("你已经成功生成了10个二次登陆验证码。", "./ShowRecoveryCodes");
        }
    }
}
