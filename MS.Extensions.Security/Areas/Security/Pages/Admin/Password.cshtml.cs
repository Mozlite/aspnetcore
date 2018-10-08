using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security;
using Mozlite.Extensions.Security.Permissions;
using MS.Extensions.Security;

namespace MS.Areas.Security.Pages.Admin
{
    /// <summary>
    /// 设置密码。
    /// </summary>
    [PermissionAuthorize(Security.Permissions.Users)]
    public class PasswordModel : ModelBase
    {
        private readonly IUserManager _userManager;

        public PasswordModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Model { get; set; }

        public class InputModel
        {
            /// <summary>
            /// 用户Id。
            /// </summary>
            public int UserId { get; set; }

            [Display(Name = "密码")]
            [Required(ErrorMessage = "{0}不能为空！")]
            [StringLength(16, ErrorMessage = "{0}的长度必须大于{2}， 小于{1}个字符！", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "确认密码")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "{0}和{1}不匹配！")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(int id)
        {
            Model = new InputModel
            {
                UserId = id
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(Model.UserId);
                var result = await _userManager.ResetPasswordAsync(user, Model.Password);
                if (result.Succeeded)
                {
                    Log($"设置了用户“{user.UserName}”的密码。");
                    return Success($"你已经成功设置了用户“{user.UserName}”的密码！");
                }
                return Error(result.ToErrorString());
            }
            return Error();
        }
    }
}