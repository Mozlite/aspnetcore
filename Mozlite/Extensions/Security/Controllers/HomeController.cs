using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public HomeController(IUserManager userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

        public async Task<IActionResult> Index(LoginUser model, string redirectUrl = null)
        {
            if (string.IsNullOrEmpty(model.UserName))
                return Error("用户名不能为空！");
            if (string.IsNullOrEmpty(model.Password))
                return Error("密码不能为空！");
            model.UserName = model.UserName.Trim();
            model.Password = model.Password.Trim();
            var result = await _signInManager.PasswordSignInAsync(model.UserName, SecurityHelper.CreatePassword(model.UserName, model.Password), model.IsRemembered, true);
            if (result.Succeeded)
            {
                return Success(new {redirectUrl});
            }
            return Error(result.ToString());
        }
    }
}