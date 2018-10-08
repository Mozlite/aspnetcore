using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security;
using MS.Extensions.Security;
using System.Threading.Tasks;
using Mozlite.Extensions.Security.Permissions;

namespace MS.Areas.Security.Pages.Admin
{
    /// <summary>
    /// 添加用户。
    /// </summary>
    [PermissionAuthorize(Security.Permissions.Users)]
    public class CreateModel : ModelBase
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;

        public CreateModel(IUserManager userManager, IRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public class InputModel
        {
            /// <summary>
            /// 登陆名称。
            /// </summary>
            [Display(Name = "登陆名称")]
            [Required(ErrorMessage = "{0}不能为空！")]
            [RegularExpression("[@a-zA-Z][a-zA-Z_0-9]{4,15}", ErrorMessage = "{0}必须以字母或者@开头，由字母和数字以及下划线组合5到16个字符组成！")]
            public string UserName { get; set; }

            public string RealName { get; set; }

            [Display(Name = "电子邮件")]
            [EmailAddress]
            public string Email { get; set; }

            [Display(Name = "密码")]
            [Required(ErrorMessage = "{0}不能为空！")]
            [StringLength(16, ErrorMessage = "{0}的长度必须大于{2}， 小于{1}个字符！", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "确认密码")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "{0}和{1}不匹配！")]
            public string ConfirmPassword { get; set; }

            public string PhoneNumber { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByNameAsync(DefaultRole.Member.NormalizedName);
                var user = new User
                {
                    RoleId = role.RoleId,
                    RoleName = role.Name,
                    UserName = Input.RealName,
                    NormalizedUserName = _userManager.NormalizeKey(Input.UserName),
                    PasswordHash = _userManager.PasswordSalt(Input.UserName, Input.Password),
                    Email = Input.Email,
                    NormalizedEmail = _userManager.NormalizeKey(Input.Email),
                    PhoneNumber = Input.PhoneNumber,
                    EmailConfirmed = !string.IsNullOrEmpty(Input.Email) && !Settings.RequiredEmailConfirmed,
                    PhoneNumberConfirmed = !string.IsNullOrEmpty(Input.PhoneNumber) && !Settings.RequiredPhoneNumberConfirmed,
                    LockoutEnabled = true
                };
                user.TwoFactorEnabled = (!string.IsNullOrEmpty(Input.Email) || !string.IsNullOrEmpty(Input.PhoneNumber)) && Settings.RequiredTwoFactorEnabled;
                var result = await _userManager.IsDuplicatedAsync(user);
                if (!result.Succeeded)
                    return Error(result.ToErrorString());
                result = await _userManager.CreateAsync(user, user.PasswordHash);
                if (result.Succeeded)
                {
                    Log($"添加了账户“{Input.UserName}”的信息！");
                    return Success("你已经成功添加了账户！");
                }
                return Error(result.ToErrorString());
            }
            return Error();
        }
    }
}