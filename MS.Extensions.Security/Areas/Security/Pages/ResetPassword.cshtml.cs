using Microsoft.AspNetCore.Mvc;
using MS.Extensions.Security;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MS.Areas.Security.Pages
{
    public class ResetPasswordModel : ModelBase
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            /// <summary>
            /// 用户名。
            /// </summary>
            [Required(ErrorMessage = "{0}不能为空！")]
            [Display(Name = "用户名")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "{0}不能为空！")]
            [Display(Name = "密码")]
            [StringLength(16, ErrorMessage = "{0}的长度必须大于{2}， 小于{1}个字符！", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "确认密码")]
            [Compare("Password", ErrorMessage = "{0}和{1}不匹配！")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "重置码")]
            [Required(ErrorMessage = "{0}不能为空！")]
            public string Code { get; set; }

        }

        private readonly IUserManager _userManager;

        public ResetPasswordModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("没有找到重置码。");
            }

            Input = new InputModel
            {
                Code = code
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByNameAsync(Input.UserName);
            if (user == null)
            {
                return ErrorPage("用户不存在，请重新输入用户名再进行密码重置！");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                Log("重置了密码！");
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}
