using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Settings;
using MozliteDemo.Extensions.Security.Areas.Security.Models;
using MozliteDemo.Extensions.Security.Properties;
using System;
using System.Threading.Tasks;

namespace MozliteDemo.Extensions.Security.Areas.Security.Controllers
{
    /// <summary>
    /// 账户相关操作。
    /// </summary>
    [Authorize]
    public class AccountController : Mozlite.Mvc.ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly ISettingsManager _settingsManager;

        /// <summary>
        /// 初始化类<see cref="AccountController"/>。
        /// </summary>
        /// <param name="userManager">用户管理接口。</param>
        /// <param name="settingsManager">配置管理接口。</param>
        public AccountController(IUserManager userManager, ISettingsManager settingsManager)
        {
            _userManager = userManager;
            _settingsManager = settingsManager;
        }

        /// <summary>
        /// 退出登录。
        /// </summary>
        [Route("logout")]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _userManager.SignOutAsync();
            EventLogger.LogUser("退出了登录。");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            return LocalRedirect("/");
        }

        /// <summary>
        /// 登录。
        /// </summary>
        [HttpPost]
        [Route("signin")]
        [AllowAnonymous]
        public async Task<IActionResult> Index(SigninUser model, string returnUrl = null)
        {
            try
            {
                var settings = await _settingsManager.GetSettingsAsync<SecuritySettings>();
#if !DEBUG
                if (settings.ValidCode && !IsValidateCode("login", model.Code))
                    return Error("验证码错误！");
#endif
                returnUrl = returnUrl ?? Url.GetDirection(settings.LoginDirection);
                model.UserName = model.UserName.Trim();
                model.Password = model.Password.Trim();

                var result = await _userManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, async user => await EventLogger.LogAsync(user.UserId, Resources.EventType, "成功登录系统。"));
                if (result.Succeeded)
                {
                    Response.Cookies.Delete("login");
                    return Success(new { url = returnUrl });
                }

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
                Logger.LogError(ex, $"账户[{model.UserName}]登录失败:{ex.Message}");
            }
            Logger.LogWarning($"账户[{model.UserName}]登录失败。");
            return Error("用户名或密码错误！");
        }
    }
}