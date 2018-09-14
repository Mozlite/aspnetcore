using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MS.Extensions.Security;

namespace MS.Areas.Security.Pages
{
    /// <summary>
    /// 确认邮件。
    /// </summary>
    [AllowAnonymous]
    public class ConfirmEmailModel : ModelBase
    {
        private readonly UserManager<User> _userManager;

        public ConfirmEmailModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"用户'{userId}'不存在！");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"用户'{userId}'确认邮件失败！");
            }

            return Page();
        }
    }
}
