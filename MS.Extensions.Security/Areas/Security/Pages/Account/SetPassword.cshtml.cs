using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MS.Extensions.Security;

namespace MS.Areas.Security.Pages.Account
{
    public class SetPasswordModel : ModelBase
    {
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {

            [Required(ErrorMessage = "{0}不能为空！")]
            [StringLength(16, ErrorMessage = "{0}的长度必须大于{2}， 小于{1}个字符！", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [DisplayName("新密码")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "确认密码")]
            [Compare("NewPassword", ErrorMessage = "{0}和{1}不匹配！")]
            public string ConfirmPassword { get; set; }
        }
        
        private readonly IUserManager _userManager;

        public SetPasswordModel(IUserManager userManager)
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
            
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                return RedirectToPage("./ChangePassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync();
            if (user == null)
            {
                return NotFound("用户不存在！");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _userManager.SignInManager.RefreshSignInAsync(user);
            Log("设置了密码。");
            return RedirectToSuccessPage("你已经成功设置了密码。");
        }
    }
}
