using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Permissions;
using MS.Extensions.Security;

namespace MS.Areas.Security.Pages.Admin
{
    /// <summary>
    /// 锁定用户。
    /// </summary>
    [PermissionAuthorize(Security.Permissions.Users)]
    public class LockoutModel : ModelBase
    {
        private readonly IUserManager _userManager;

        public LockoutModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public class InputModel
        {
            /// <summary>
            /// 用户Id。
            /// </summary>
            public int UserId { get; set; }

            public DateTimeOffset LockoutEnd { get; set; }

            public string Reason { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public void OnGet(int id)
        {
            Input = new InputModel
            {
                UserId = id,
                LockoutEnd = DateTimeOffset.Now.AddDays(1)
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _userManager.LockoutAsync(Input.UserId, Input.LockoutEnd))
            {
                var user = await _userManager.FindByIdAsync(Input.UserId);
                var message = $"锁定了用户“{user.UserName}”到{Input.LockoutEnd}";
                if (!string.IsNullOrEmpty(Input.Reason))
                    message += $"（原因：{Input.Reason}）!";
                else
                    message += "!";
                Log(message);
                return Success($"你已经成功锁定了用户“{user.UserName}”！");
            }
            return Error("锁定用户失败！");
        }

        public async Task<IActionResult> OnPostUnlockAsync(int id)
        {
            if (await _userManager.LockoutAsync(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                Log($"解锁了用户“{user.UserName}”的信息！");
                return Success($"你已经成功解锁了用户“{user.UserName}”！");
            }
            return Error("解锁用户失败！");
        }
    }
}