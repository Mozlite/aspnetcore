using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MS.Extensions.Security;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MS.Areas.Security.Pages.Account
{
    /// <summary>
    /// 删除数据，并关闭账户。
    /// </summary>
    public class DeletePersonalDataModel : ModelBase
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "密码")]
            [Required(ErrorMessage = "{0}不能为空！")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        private readonly IUserManager _userManager;

        public DeletePersonalDataModel(IUserManager userManager)
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

            RequirePassword = !string.IsNullOrEmpty(user.PasswordHash);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            RequirePassword = !string.IsNullOrEmpty(user.PasswordHash);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "密码错误！");
                    return Page();
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"删除用户“{user.UserName}”数据失败！");
            }

            await _userManager.SignOutAsync();

            Logger.LogInformation("用户“{UserName}”自己关闭了账户！", user.UserName);

            return Redirect("~/");
        }
    }
}
