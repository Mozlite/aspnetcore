using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security;
using Mozlite.Extensions.Security.Permissions;
using Mozlite.Mvc.Logging;
using MS.Extensions.Security;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MS.Areas.Security.Pages.Admin
{
    [PermissionAuthorize(Security.Permissions.Users)]
    public class EditModel : ModelBase
    {
        private readonly IUserManager _userManager;

        public EditModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public class InputModel
        {
            public InputModel() { }

            public InputModel(User currentUser)
            {
                UserId = currentUser.UserId;
                UserName = currentUser.UserName;
                PhoneNumber = currentUser.PhoneNumber;
                Email = currentUser.Email;
            }

            /// <summary>
            /// 用户Id。
            /// </summary>
            public int UserId { get; set; }

            [Required(ErrorMessage = "姓名不能为空！")]
            public string UserName { get; set; }

            public string PhoneNumber { get; set; }

            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task OnGetAsync(int id)
        {
            Input = new InputModel(await _userManager.FindByIdAsync(id));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(Input.UserId);
                if (user == null)
                    return Error("未找到任何关联用户！");
                var context = LogContext.Create(user, Localizer);
                if (user.UserName != Input.UserName)
                {
                    user.UserName = Input.UserName;
                    var duplicated = await _userManager.IsDuplicatedAsync(user);
                    if (!duplicated.Succeeded)
                        return Error(duplicated.ToErrorString());
                }

                if (user.Email != Input.Email)
                {
                    user.Email = Input.Email;
                    user.NormalizedEmail = _userManager.NormalizeKey(Input.Email);
                    user.EmailConfirmed = !string.IsNullOrEmpty(Input.Email) && !Settings.RequiredEmailConfirmed;
                }

                if (user.PhoneNumber != Input.PhoneNumber)
                {
                    user.PhoneNumber = Input.PhoneNumber;
                    user.PhoneNumberConfirmed = !string.IsNullOrEmpty(Input.PhoneNumber) && !Settings.RequiredPhoneNumberConfirmed;
                }

                if (context.Diff(user))
                {
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        Log($"更新了用户“{user.UserName}”的信息：{context}");
                        return Success("你已经成功更新了用户信息！");
                    }
                    return Error(result.ToErrorString());
                }
                return Success("你已经成功更新了用户信息！");
            }
            return Error();
        }
    }
}