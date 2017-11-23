using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Security.Models;
using Mozlite.Extensions.Security.Services;
using Mozlite.Extensions.Security.ViewModels;

namespace Mozlite.Extensions.Security.Controllers
{
    /// <summary>
    /// 用户登入登出，获取密码等等试图控制器。
    /// </summary>
    public class HomeController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IActivityManager _activityManager;

        public HomeController(IUserManager userManager, SignInManager<User> signInManager, ILogger<HomeController> logger, IActivityManager activityManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _activityManager = activityManager;
        }

        /// <summary>
        /// 登入页面。
        /// </summary>
        /// <param name="redirectUrl">登入成功后转向页面。</param>
        /// <returns>返回试图结果。</returns>
        [Route("login")]
        public IActionResult Index(string redirectUrl = null)
        {
            ViewBag.RedirectUrl = redirectUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginUser model, string redirectUrl = null)
        {
            if (string.IsNullOrEmpty(model.UserName))
                return Error("用户名不能为空！");
            if (string.IsNullOrEmpty(model.Password))
                return Error("密码不能为空！");

#if !DEBUG
                if (string.IsNullOrWhiteSpace(model.Code) || !IsValidateCode("login", model.Code))
                    return Error("验证码错误！");
#endif

            model.UserName = model.UserName.Trim();
            model.Password = model.Password.Trim();
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, SecurityHelper.CreatePassword(model.UserName, model.Password), model.IsRemembered, true);
                if (result.Succeeded)
                {
                    await _userManager.SignInSuccessAsync(model.UserName);
                    await _activityManager.CreateAsync("成功登入系统.");
                    return Success(new { redirectUrl });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(SecuritySettings.EventId, $"账户[{model.UserName}]被锁定。");
                    return Error("账户被锁定！");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(SecuritySettings.EventId, ex, $"账户[{model.UserName}]登入失败:{ex.Message}");
            }
            _logger.LogWarning(SecuritySettings.EventId, $"账户[{model.UserName}]登入失败。");
            return Error("用户名或密码错误！");
        }
        
        /// <summary>
        /// 退出登入。
        /// </summary>
        /// <returns>退出登入，并转向首页。</returns>
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _activityManager.CreateAsync("成功推出系统.");
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }
    }
}