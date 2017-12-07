using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Security.Activities;
using Mozlite.Extensions.Security.DisallowNames;
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
        private readonly INameManager _nameManager;

        public HomeController(IUserManager userManager, SignInManager<User> signInManager, ILogger<HomeController> logger, INameManager nameManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _nameManager = nameManager;
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
                    _logger.LogUserInformation("成功登入系统.");
                    return Success(new { redirectUrl });
                }
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToAction(nameof(SendCode),
                //        new { ReturnUrl = Request.GetDisplayUrl(), model.RememberMe });
                //}
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(SecuritySettings.EventId, $"账户[{model.UserName}]被锁定。");
                    return Error("账户被锁定！");
                }
                if (result.IsNotAllowed)
                {
                    return Error("账户未激活，请打开你注册时候的邮箱进行验证！如果没有收到邮件，<span style=\"cursor:pointer;\" onclick=\"$ajax('/user/confirm',{name:'" + model.UserName + "'});\">点击重新发送验证邮件</span>...");
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
        /// 创建账户。
        /// </summary>
        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUser model)
        {
            if (string.IsNullOrEmpty(model.UserName))
                return Error("用户名不能为空！");
            if (string.IsNullOrEmpty(model.Password))
                return Error("密码不能为空！");
            if (string.IsNullOrEmpty(model.Email))
                return Error("电子邮件不能为空！");

#if !DEBUG
                if (string.IsNullOrWhiteSpace(model.Code) || !IsValidateCode("register", model.Code))
                    return Error("验证码错误！");
#endif
            model.UserName = model.UserName.Trim();
            model.Password = model.Password.Trim();
            if (model.Password != model.ConfirmPassword)
                return Error("密码和确认密码不匹配！");

            if (await _nameManager.IsDisallowedAsync(model.UserName))
                return Error("用户名被禁用，请使用其他用户名！");

            if (await _userManager.CheckUserNameAsync(model.UserName))
                return Error("用户已经存在，请使用其他用户名！");

            var user = new User();
            user.UserName = model.UserName;
            user.NickName = model.UserName;
            user.Email = model.Email;
            var result = await _userManager.CreateAsync(user, SecurityHelper.CreatePassword(model.UserName, model.Password), IdentitySettings.Register);
            if (result.Succeeded)
            {
                try
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = ActionUrl("ConfirmEmail", new { userId = user.UserId, code });
                    //await
                    //    _emailSender.SendEmailAsync(user, Resources.Email_ActiveAccount,
                    //        Resources.Email_ActiveAccount_Body.ReplaceBy(
                    //            kw =>
                    //            {
                    //                kw.Add("name", user.NickName);
                    //                kw.Add("link", callbackUrl);
                    //            }));
                    _logger.LogUserInformation("注册了账户[{0}].", user.UserName);
                    return Success("一封激活邮件已经发送到你的邮箱！该激活链接24小时内有效！");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "邮件发送失败：" + exception.Message);
                    return Warning("账户已经成功注册，但是激活邮件发送失败，请联系网站管理员！");
                }
            }
            _logger.LogWarning("注册账户[{0}]失败.", user.UserName);
            var error = result.Errors.FirstOrDefault();
            if (error != null)
                return Error(Resources.ResourceManager.GetString(error.Code));
            return Error("注册失败！");
        }


        /// <summary>
        /// 发送激活邮件。
        /// </summary>
        [HttpPost]
        [Route("user/confirm")]
        public async Task<IActionResult> ConfirmEmail(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = ActionUrl("ConfirmEmail", new { userId = user.UserId, code });
            //await _emailSender.SendEmailAsync(user, Resources.Email_ActiveAccount, Resources.Email_ActiveAccount_Body.ReplaceBy(
            //    kw =>
            //    {
            //        kw.Add("name", user.NickName);
            //        kw.Add("link", callbackUrl);
            //    }));
            return Success("你已经成功注册账户，一封激活邮件已经发送到你的邮箱！该激活链接24小时内有效！");
        }

        /// <summary>
        /// 激活邮件。
        /// </summary>
        [Route("user/confirm")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>
        /// 退出登入。
        /// </summary>
        /// <returns>退出登入，并转向首页。</returns>
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            _logger.LogUserInformation("成功退出系统.");
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }
    }
}