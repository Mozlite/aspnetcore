using System;
using System.Linq;
using Mozlite.Mvc.Messages;
using Mozlite.Extensions.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Mozlite.Extensions.Security;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Extensions.Logging;
using Mozlite.Extensions.Security.Permissions;
using Mozlite.Mvc.Controllers;
using Mozlite.Mvc.TagHelpers.Common;

namespace Mozlite.Mvc
{
    /// <summary>
    /// 页面模型基类。
    /// </summary>
    public abstract class ModelBase : PageModel
    {
        #region common
        private Version _version;
        /// <summary>
        /// 当前程序的版本。
        /// </summary>
        public Version Version => _version ?? (_version = Assembly.GetEntryAssembly().GetName().Version);

        private ILocalizer _localizer;
        /// <summary>
        /// 本地化接口。
        /// </summary>
        public ILocalizer Localizer => _localizer ?? (_localizer = GetRequiredService<ILocalizer>());


        private ILogger _logger;
        /// <summary>
        /// 日志接口。
        /// </summary>
        protected virtual ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = GetRequiredService<ILoggerFactory>()
                        .CreateLogger(GetType());
                }
                return _logger;
            }
        }

        /// <summary>
        /// 实例化一个日志存储对象，用于存储修改前得实例。
        /// </summary>
        /// <param name="storage">当前修改模型修改前得实例对象。</param>
        /// <returns>返回日志实例存储对象。</returns>
        protected LogStorage CreateStorage(object storage)
        {
            var log = new LogStorage(storage);
            log.Localizer = Localizer;
            return log;
        }

        private int _pageIndex = -1;
        /// <summary>
        /// 当前页码。
        /// </summary>
        public virtual int PageIndex
        {
            get
            {
                if (_pageIndex == -1)
                {
                    string page;
                    if (RouteData.Values.TryGetValue("pi", out var pobject))
                        page = pobject.ToString();
                    else
                        page = Request.Query["pi"];
                    if (!int.TryParse(page, out _pageIndex))
                        _pageIndex = 1;
                    if (_pageIndex < 1)
                        _pageIndex = 1;
                }
                return _pageIndex;
            }
        }

        /// <summary>
        /// 判断验证码。
        /// </summary>
        /// <param name="key">当前唯一键。</param>
        /// <param name="code">验证码。</param>
        /// <returns>返回判断结果。</returns>
        protected virtual bool IsValidateCode(string key, string code)
        {
            if (string.IsNullOrEmpty(code) || !Request.Cookies.TryGetValue(key, out var value))
                return false;
            code = Verifiers.Hashed(code);
            return string.Equals(value, code, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 获取注册的服务对象。
        /// </summary>
        /// <typeparam name="TService">服务类型或者接口。</typeparam>
        /// <returns>返回当前服务的实例对象。</returns>
        protected TService GetService<TService>()
        {
            return HttpContext.RequestServices.GetService<TService>();
        }

        /// <summary>
        /// 获取已经注册的服务对象。
        /// </summary>
        /// <typeparam name="TService">服务类型或者接口。</typeparam>
        /// <returns>返回当前服务的实例对象。</returns>
        protected TService GetRequiredService<TService>()
        {
            return HttpContext.RequestServices.GetRequiredService<TService>();
        }
        #endregion

        #region users
        /// <summary>
        /// 判断当前用户是否有权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        public bool HasPermission(string permissionName)
        {
            if (string.IsNullOrEmpty(permissionName))
                return false;
            return GetRequiredService<IPermissionManager>().Exist(permissionName);
        }

        /// <summary>
        /// 判断当前用户是否有权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        public async Task<bool> HasPermissionAsync(string permissionName)
        {
            if (string.IsNullOrEmpty(permissionName))
                return false;
            return await GetRequiredService<IPermissionManager>().ExistAsync(permissionName);
        }

        /// <summary>
        /// 判断当前用户是否有权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsAuthorized(string permissionName)
        {
            return GetRequiredService<IPermissionManager>().IsAuthorized(permissionName);
        }

        /// <summary>
        /// 判断当前用户是否有权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        public Task<bool> IsAuthorizedAsync(string permissionName)
        {
            return GetRequiredService<IPermissionManager>().IsAuthorizedAsync(permissionName);
        }

        /// <summary>
        /// 是否已经登入。
        /// </summary>
        public bool IsAuthenticated => User.Identity.IsAuthenticated;

        private int _userId = -1;
        /// <summary>
        /// 当前用户ID。
        /// </summary>
        public int UserId
        {
            get
            {
                if (_userId == -1)
                {
                    _userId = User.GetUserId();
                }
                return _userId;
            }
        }

        private string _userName;

        /// <summary>
        /// 当前用户名称。
        /// </summary>
        public string UserName => _userName ?? (_userName = User.GetUserName());
        #endregion

        #region jsons
        /// <summary>
        /// 返回消息的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Info(string message = null)
        {
            return Json(BsType.Info, message);
        }

        /// <summary>
        /// 返回消息的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="data">数据实例对象。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Info<TData>(string message, TData data)
        {
            return Json(BsType.Info, message, data);
        }

        /// <summary>
        /// 返回警告的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Warning(string message = null)
        {
            return Json(BsType.Warning, message);
        }

        /// <summary>
        /// 返回警告的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="data">数据实例对象。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Warning<TData>(string message, TData data)
        {
            return Json(BsType.Warning, message, data);
        }

        /// <summary>
        /// 返回错误的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Error(string message = null)
        {
            return Json(BsType.Danger, message);
        }

        /// <summary>
        /// 返回错误的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="data">数据实例对象。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Error<TData>(string message, TData data)
        {
            return Json(BsType.Danger, message, data);
        }

        /// <summary>
        /// 返回成功的JSON对象。
        /// </summary>
        /// <param name="affected">是否有执行。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Success(bool affected = true)
        {
            return Success(new { affected });
        }

        /// <summary>
        /// 返回成功的JSON对象。
        /// </summary>
        /// <param name="data">客户数据对象。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Success<TData>(TData data)
        {
            return Json(BsType.Success, null, data);
        }

        /// <summary>
        /// 返回成功的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Success(string message)
        {
            return Json(BsType.Success, message);
        }

        /// <summary>
        /// 返回成功的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="data">数据实例对象。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Success<TData>(string message, TData data)
        {
            return Json(BsType.Success, message, data);
        }

        private IActionResult Json(BsType type, string message)
        {
            if (!ModelState.IsValid)
            {
                var dic = new Dictionary<string, string>();
                foreach (var key in ModelState.Keys)
                {
                    var error = ModelState[key].Errors.FirstOrDefault()?.ErrorMessage;
                    if (string.IsNullOrEmpty(key))
                        message = message ?? error;
                    else
                        dic[key] = error;
                }
                return Json(type, message, dic);
            }
            return new JsonResult(new JsonMesssage(type, message));
        }

        private IActionResult Json<TData>(BsType type, string message, TData data)
        {
            return new JsonResult(new JsonMesssage<TData>(type, message, data));
        }

        /// <summary>
        /// 返回JSON试图结果。
        /// </summary>
        /// <param name="result">数据结果。</param>
        /// <param name="args">参数。</param>
        /// <returns>返回JSON试图结果。</returns>
        protected IActionResult Json(DataResult result, params object[] args)
        {
            if (result.Succeed())
                return Json(BsType.Success, result.ToString(args));
            return Json(BsType.Danger, result.ToString(args));
        }

        /// <summary>
        /// 模型错误。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <param name="args">参数。</param>
        /// <returns>返回错误实例。</returns>
        protected IActionResult ModelError(string message, params object[] args)
        {
            ModelState.AddModelError("", string.Format(message, args));
            return Error();
        }

        /// <summary>
        /// 模型错误。
        /// </summary>
        /// <param name="key">属性名称。</param>
        /// <param name="message">错误消息。</param>
        /// <param name="args">参数。</param>
        /// <returns>返回错误实例。</returns>
        protected IActionResult ModelError(string key, string message, params object[] args)
        {
            if (!key.StartsWith("Model."))
                key = $"Model.{key}";
            ModelState.AddModelError(key, string.Format(message, args));
            return Error();
        }

        /// <summary>
        /// 返回JSON试图结果。
        /// </summary>
        /// <param name="result">数据结果。</param>
        /// <returns>返回JSON试图结果。</returns>
        protected IActionResult Error(IdentityResult result)
        {
            var errors = result.Errors.Select(x => x.Description).ToList();
            return Json(BsType.Danger, string.Join(", ", errors));
        }
        #endregion

        #region helpers
        private Browser? _browser;
        /// <summary>
        /// 浏览器类型。
        /// </summary>
        public Browser Browser
        {
            get
            {
                if (_browser == null)
                {
                    string browser = Request.Headers["User-Agent"];
                    if (browser.IndexOf("Edge") != -1)
                        _browser = Browser.Edge;
                    else if (browser.IndexOf("MSIE") != -1 || browser.IndexOf("rv:") != -1)
                        _browser = Browser.IE;
                    else if (browser.IndexOf("Firefox") != -1)
                        _browser = Browser.Firefox;
                    else if (browser.IndexOf("Chrome") != -1)
                        _browser = Browser.Chrome;
                    else if (browser.IndexOf("Safari") != -1)
                        _browser = Browser.Safari;
                    else
                        _browser = Browser.Unknown;
                }
                return _browser.Value;
            }
        }
        #endregion
    }

    /// <summary>
    /// 页面模型基类，在POST表单页面中使用，自动绑定到表单实例中。
    /// </summary>
    /// <typeparam name="TModel">模型实例类型。</typeparam>
    public abstract class ModelBase<TModel> : ModelBase, IModelable
    {
        /// <summary>
        /// 当前模型实例。
        /// </summary>
        [BindProperty]
        public TModel Model { get; set; }

        object IModelable.Model => Model;
    }
}