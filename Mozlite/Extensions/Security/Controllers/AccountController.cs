using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Services;
using Mozlite.Extensions.Security.ViewModels;

namespace Mozlite.Extensions.Security.Controllers
{
    /// <summary>
    /// 账户登陆后的控制器。
    /// </summary>
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public AccountController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Menu()
        {
            return View();
        }

        public IActionResult Profile()
        {
            var user = _userManager.GetUser();
            return View(_userManager.GetProfile(user));
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
                return ErrorView("原始密码不能为空！", model);

            if (string.IsNullOrWhiteSpace(model.NewPassword))
                return ErrorView("新密码不能为空！", model);

            model.NewPassword = model.Password.Trim();
            if (model.Password != model.ConfirmPassword?.Trim())
                return ErrorView("新密码和确认密码不一致！", model);

            var user = _userManager.GetUser();
            var result =
                await
                    _userManager.ChangePasswordAsync(user,
                        SecurityHelper.CreatePassword(user.UserName, model.Password),
                        SecurityHelper.CreatePassword(user.UserName, model.NewPassword));
            if (result.Succeeded)
                return SuccessView("你已经成功更新了密码，下次登陆时候需要使用新密码进行登陆！", model);

            var error = result.Errors.FirstOrDefault();
            if (error != null)
                return ErrorView(error.Description);
            return ErrorView("修改密码失败！", model);
        }
    }
}