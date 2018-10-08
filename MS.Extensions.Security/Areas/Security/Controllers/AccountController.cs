using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Security.Activities;
using MS.Areas.Security.Models;
using MS.Extensions.Security;
using MS.Extensions.Security.Activities;
using System;
using System.Threading.Tasks;
using ControllerBase = Mozlite.Mvc.ControllerBase;

namespace MS.Areas.Security.Controllers
{
    /// <summary>
    /// 账户相关操作。
    /// </summary>
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IUserManager _userManager;
        /// <summary>
        /// 初始化类<see cref="AccountController"/>。
        /// </summary>
        /// <param name="userManager">用户管理接口。</param>
        public AccountController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// 退出登陆。
        /// </summary>
        [Route("logout")]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _userManager.SignOutAsync();
            Logger.Info(SecuritySettings.EventId, "退出登陆。");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            return LocalRedirect("/");
        }

        /// <summary>
        /// 登陆。
        /// </summary>
        [HttpPost]
        [Route("signin")]
        [AllowAnonymous]
        public async Task<IActionResult> Index(SigninUser model, string returnUrl = null)
        {
            try
            {
#if !DEBUG
                if (!IsValidateCode("login", model.Code))
                    return Error("验证码错误！");
#endif
                model.UserName = model.UserName.Trim();
                model.Password = model.Password.Trim();
                var result = await _userManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe,
                    async user =>
                    {
                        await GetRequiredService<IActivityManager>().CreateAsync(SecuritySettings.EventId, "成功登入系统.", user.UserId);
                    });
                if (result.Succeeded)
                    return Success(new { url = returnUrl ?? "/" });

                if (result.RequiresTwoFactor)
                    return Success(new { reurl = Url.Page("/LoginWith2fa", new { model.RememberMe, returnUrl, area = SecuritySettings.ExtensionName }) });

                if (result.IsLockedOut)
                {
                    Logger.LogWarning($"账户[{model.UserName}]被锁定。");
                    return Error("账户被锁定！");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"账户[{model.UserName}]登陆失败:{ex.Message}");
            }
            Logger.LogWarning($"账户[{model.UserName}]登陆失败。");
            return Error("用户名或密码错误！");
        }
    }
}