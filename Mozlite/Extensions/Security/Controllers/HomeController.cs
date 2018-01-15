using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.DisallowNames;
using Mozlite.Extensions.Messages;
using Mozlite.Extensions.Messages.Services;
using Mozlite.Extensions.Security.Activities;
using Mozlite.Extensions.Security.Models;
using Mozlite.Extensions.Security.Properties;
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
        private readonly IDisallowNameManager _nameManager;
        private readonly IMessageManager _messageManager;
        private readonly IActivityManager _activityManager;

        public HomeController(IUserManager userManager, 
            SignInManager<User> signInManager, 
            ILogger<HomeController> logger, 
            IDisallowNameManager nameManager, 
            IMessageManager messageManager, 
            IActivityManager activityManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _nameManager = nameManager;
            _messageManager = messageManager;
            _activityManager = activityManager;
        }
        
        /// <summary>
        /// 登入页面。
        /// </summary>
        /// <param name="redirectUrl">登入成功后转向页面。</param>
        /// <returns>返回试图结果。</returns>
        [Route("login")]
        public IActionResult Login(string redirectUrl = null)
        {
            ViewBag.RedirectUrl = redirectUrl;
            return View();
        }

        [HttpPost]
        [Route("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUser model, string redirectUrl = null)
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
                    var userId = await _userManager.GetUserIdAsync(model.UserName);
                    await _userManager.SignInSuccessAsync(userId);
                    await _activityManager.CreateAsync("成功登入系统.", userId);
                    return Success(new { redirectUrl = redirectUrl ?? "/dashboard" });
                }
                if (result.RequiresTwoFactor)
                {
                    return Success(new
                    {
                        redirectUrl = Url.RouteUrl(nameof(SendCode),
                        new { ReturnUrl = Request.GetDisplayUrl(), model.IsRemembered })
                    });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(SecuritySettings.EventId, $"账户[{model.UserName}]被锁定。");
                    return Error("账户被锁定！");
                }
                if (result.IsNotAllowed)
                {
                    return Error("账户未激活，请打开你注册时候的邮箱进行验证！如果没有收到邮件，<a href=\"javascript:;\" onclick=\"$ajax('/user/confirm',{name:'" + model.UserName + "'});\">点击重新发送验证邮件</a>...");
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
        [Route("register")]
        [ValidateAntiForgeryToken]
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
                    await _messageManager.SendEmailAsync(user.UserId, user.Email, Resources.Email_ActiveAccount,
                            Resources.Email_ActiveAccount_Body.ReplaceBy(
                                kw =>
                                {
                                    kw.Add("name", user.NickName);
                                    kw.Add("link", callbackUrl);
                                }));
                    await _activityManager.CreateAsync("注册了账户.", user.UserId);
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
            await _messageManager.SendEmailAsync(user.UserId, user.Email, Resources.Email_ActiveAccount, Resources.Email_ActiveAccount_Body.ReplaceBy(
                kw =>
                {
                    kw.Add("name", user.NickName);
                    kw.Add("link", callbackUrl);
                }));
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

        /// <summary>
        /// 发送验证码。
        /// </summary>
        [Route("user/sendcode")]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>
        /// 发送验证码。
        /// </summary>
        [HttpPost]
        [Route("user/sendcode")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "你的验证码是: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _messageManager.SendEmailAsync(user.UserId, user.Email, "[$site;]验证码", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _messageManager.SendSMSAsync(user.UserId, user.PhoneNumber, message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        /// <summary>
        /// 验证激活码。
        /// </summary>
        [Route("user/verifycode")]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>
        /// 验证激活码。
        /// </summary>
        [HttpPost]
        [Route("user/verifycode")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "用户已经被锁定！");
                return View("Lockout");
            }
            ModelState.AddModelError(string.Empty, "验证码错误！");
            return View(model);
        }
    }
}