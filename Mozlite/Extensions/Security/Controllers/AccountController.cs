using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Security.Models;
using Mozlite.Extensions.Security.Services;
using Mozlite.Extensions.Security.ViewModels;
using Mozlite.Extensions.Security.Activities;

namespace Mozlite.Extensions.Security.Controllers
{
    /// <summary>
    /// 账户登陆后的控制器。
    /// </summary>
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserManager userManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _logger = logger;
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

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfile model)
        {
            model.Id = UserId;
            if (await _userManager.SaveAsync(model))
            {
                _logger.LogUserInformation("编辑了用户资料信息！");
                return SuccessView("恭喜你，你已经成功更新了用户资料！", model);
            }
            return ErrorView("很抱歉，更新用户资料错误！", model);
        }

        public IActionResult Avatar()
        {
            return View(_userManager.GetUser());
        }

        [HttpPost]
        public async Task<IActionResult> Avatar(IFormFile file)
        {
            var result = await _userManager.UploadAvatarAsync(file, UserId);
            if (result.Succeeded)
            {
                return Success(new { result.Url });
            }
            return Error(result.Message);
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
            {
                _logger.LogUserInformation("更改了新密码为：{0}", model.NewPassword);
                return SuccessView("你已经成功更新了密码，下次登陆时候需要使用新密码进行登陆！", model);
            }
            var error = result.Errors.FirstOrDefault();
            if (error != null)
                return ErrorView(error.Description);
            return ErrorView("修改密码失败！", model);
        }
    }
}