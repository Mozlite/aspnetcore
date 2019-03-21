using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions;
using Mozlite.Extensions.Messages;
using Mozlite.Extensions.Messages.Notifications;
using Mozlite.Extensions.Security;
using Mozlite.Extensions.Security.Events;
using Mozlite.Extensions.Security.Permissions;
using Mozlite.Extensions.Security.Stores;
using Mozlite.Extensions.Storages;
using Mozlite.Extensions.Storages.Apis;
using Mozlite.Extensions.Storages.Excels;
using Mozlite.Mvc.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Mozlite.Extensions.Settings;

namespace Mozlite.Mvc
{
    /// <summary>
    /// 控制器基类。
    /// </summary>
    public abstract class ControllerBase : Controller
    {
        #region commons
        private Version _version;
        /// <summary>
        /// 当前程序的版本。
        /// </summary>
        public Version Version => _version ?? (_version = Cores.Version);

        private ILocalizer _localizer;
        /// <summary>
        /// 本地化接口。
        /// </summary>
        public ILocalizer Localizer => _localizer ?? (_localizer = GetRequiredService<ILocalizer>());

        private ILogger _logger;
        /// <summary>
        /// 日志接口。
        /// </summary>
        protected virtual ILogger Logger => _logger ?? (_logger = GetRequiredService<ILoggerFactory>().CreateLogger(GetType()));

        private IEventLogger _eventLogger;
        /// <summary>
        /// 日志接口。
        /// </summary>
        protected virtual IEventLogger EventLogger => _eventLogger ?? (_eventLogger = GetRequiredService<IEventLogger>());

        private string _controllerName;
        /// <summary>
        /// 获取当前控制器名称。
        /// </summary>
        protected string ControllerName => _controllerName ?? (_controllerName = ControllerContext.ActionDescriptor.ControllerName);

        private string _actionName;
        /// <summary>
        /// 获取当前试图名称。
        /// </summary>
        protected string ActionName => _actionName ?? (_actionName = ControllerContext.ActionDescriptor.ActionName);

        private string _areaName;

        /// <summary>
        /// 获取当前区域名称。
        /// </summary>
        protected string AreaName
        {
            get
            {
                if (_actionName == null)
                {
                    ControllerContext.ActionDescriptor.RouteValues.TryGetValue("area", out _areaName);
                }
                return _areaName;
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
        protected TService GetService<TService>() => HttpContext.RequestServices.GetService<TService>();

        /// <summary>
        /// 获取已经注册的服务对象。
        /// </summary>
        /// <typeparam name="TService">服务类型或者接口。</typeparam>
        /// <returns>返回当前服务的实例对象。</returns>
        protected TService GetRequiredService<TService>() => HttpContext.RequestServices.GetRequiredService<TService>();

        private INotifier _notifier;
        /// <summary>
        /// 通知接口实例。
        /// </summary>
        protected INotifier Notifier => _notifier ?? (_notifier = GetRequiredService<INotifier>());

        private IObjectDiffer _differ;
        /// <summary>
        /// 对象属性变更实例。
        /// </summary>
        protected IObjectDiffer Differ => _differ ?? (_differ = GetRequiredService<IObjectDiffer>());

        private ISettingDictionaryManager _settingDictionaryManager;
        /// <summary>
        /// 获取字典值。
        /// </summary>
        /// <param name="path">标识。</param>
        /// <returns>返回字典值。</returns>
        public string GetDictionaryValue(string path) =>
            (_settingDictionaryManager ?? (_settingDictionaryManager = GetRequiredService<ISettingDictionaryManager>()))
            .GetOrAddSettings(path);
        #endregion

        #region views
        /// <summary>
        /// 返回警告的试图对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="model">模型实例对象。</param>
        /// <param name="action">转向试图。</param>
        /// <returns>返回试图结果。</returns>
        protected IActionResult WarningView(string message, object model = null, string action = null)
        {
            return View(StatusType.Warning, message, model, action);
        }

        /// <summary>
        /// 返回信息的试图对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="model">模型实例对象。</param>
        /// <param name="action">转向试图。</param>
        /// <returns>返回试图结果。</returns>
        protected IActionResult InfoView(string message, object model = null, string action = null)
        {
            return View(StatusType.Info, message, model, action);
        }

        /// <summary>
        /// 返回错误的试图对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="model">模型实例对象。</param>
        /// <param name="action">转向试图。</param>
        /// <returns>返回试图结果。</returns>
        protected IActionResult ErrorView(string message, object model = null, string action = null)
        {
            return View(StatusType.Danger, message, model, action);
        }

        /// <summary>
        /// 返回成功的试图对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="model">模型实例对象。</param>
        /// <param name="action">转向试图。</param>
        /// <returns>返回试图结果。</returns>
        protected IActionResult SuccessView(string message, object model = null, string action = null)
        {
            return View(StatusType.Success, message, model, action);
        }

        /// <summary>
        /// 显示消息。
        /// </summary>
        /// <param name="result">操作接口。</param>
        /// <param name="model">模型实例对象。</param>
        /// <param name="args">参数。</param>
        protected IActionResult View(DataResult result, object model, params object[] args)
        {
            if (result.Succeed())
                return View(StatusType.Success, result.ToString(args), model);
            return View(StatusType.Danger, result.ToString(args), model);
        }

        /// <summary>
        /// 显示消息。
        /// </summary>
        /// <param name="result">操作接口。</param>
        /// <param name="url">转向地址。</param>
        /// <param name="model">模型实例对象。</param>
        /// <param name="args">参数。</param>
        protected IActionResult View(DataResult result, string url, object model, params object[] args)
        {
            if (result.Succeed())
                return View(StatusType.Success, result.ToString(args), model, url);
            return View(StatusType.Danger, result.ToString(args), model);
        }

        private IActionResult View(StatusType type, string message, object model, string url = null)
        {
            var statusMessage = new StatusMessage(TempData);
            statusMessage.Type = type;
            statusMessage.Message = message;
            // 不是网址
            if (url != null)
            {
                if (url.IndexOf('/') == -1)
                    url = Url.Action(url, ControllerName, new { area = AreaName });
                return Redirect(url);
            }
            return View(model);
        }
        #endregion

        #region jsons
        /// <summary>
        /// 返回消息的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Info(string message = null)
        {
            return Json(StatusType.Info, message);
        }

        /// <summary>
        /// 返回消息的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="data">数据实例对象。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Info<TData>(string message, TData data)
        {
            return Json(StatusType.Info, message, data);
        }

        /// <summary>
        /// 返回警告的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Warning(string message = null)
        {
            return Json(StatusType.Warning, message);
        }

        /// <summary>
        /// 返回警告的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="data">数据实例对象。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Warning<TData>(string message, TData data)
        {
            return Json(StatusType.Warning, message, data);
        }

        /// <summary>
        /// 返回错误的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Error(string message = null)
        {
            return Json(StatusType.Danger, message);
        }

        /// <summary>
        /// 返回错误的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="data">数据实例对象。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Error<TData>(string message, TData data)
        {
            return Json(StatusType.Danger, message, data);
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
            return Json(StatusType.Success, null, data);
        }

        /// <summary>
        /// 返回成功的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Success(string message)
        {
            return Json(StatusType.Success, message);
        }

        /// <summary>
        /// 返回成功的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="data">数据实例对象。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Success<TData>(string message, TData data)
        {
            return Json(StatusType.Success, message, data);
        }

        private IActionResult Json(StatusType type, string message)
        {
            if (type != StatusType.Success && !ModelState.IsValid)
            {
                type = StatusType.Danger;
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
            return Json(new JsonMesssage(type, message));
        }

        private IActionResult Json<TData>(StatusType type, string message, TData data)
        {
            return Json(new JsonMesssage<TData>(type, message, data));
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
                return Json(StatusType.Success, result.ToString(args));
            return Json(StatusType.Danger, result.ToString(args));
        }

        /// <summary>
        /// 返回JSON试图结果。
        /// </summary>
        /// <param name="result">数据结果。</param>
        /// <returns>返回JSON试图结果。</returns>
        protected IActionResult Error(IdentityResult result)
        {
            var errors = result.Errors.Select(x => x.Description).ToList();
            return Json(StatusType.Danger, string.Join(", ", errors));
        }
        #endregion

        #region urls
        /// <summary>
        /// 获取当前Action对应的地址。
        /// </summary>
        /// <param name="action">试图。</param>
        /// <param name="values">路由对象。</param>
        /// <returns>返回当前Url地址。</returns>
        protected string ActionUrl(string action, object values = null)
        {
            return ActionUrl(action, ControllerName, values);
        }

        /// <summary>
        /// 获取当前Action对应的地址。
        /// </summary>
        /// <param name="action">试图。</param>
        /// <param name="controller">控制器。</param>
        /// <param name="values">路由对象。</param>
        /// <returns>返回当前Url地址。</returns>
        protected string ActionUrl(string action, string controller, object values = null)
        {
            if (values == null)
                return Url.Action(action, controller, AreaName == null ? null : new { area = AreaName }, Request.Scheme);
            var routes = new RouteValueDictionary(values);
            if (routes.ContainsKey("area") || AreaName == null)
                return Url.Action(action, controller, routes, Request.Scheme);
            routes.Add("area", AreaName);
            return Url.Action(action, controller, routes, Request.Scheme);
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
            return GetRequiredService<IPermissionManager>().Exist(permissionName);
        }

        /// <summary>
        /// 判断当前用户是否有权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        public Task<bool> HasPermissionAsync(string permissionName)
        {
            return GetRequiredService<IPermissionManager>().ExistAsync(permissionName);
        }

        /// <summary>
        /// 判断当前用户是否有权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        protected bool IsAuthorized(string permissionName)
        {
            return GetRequiredService<IPermissionManager>().IsAuthorized(permissionName);
        }

        /// <summary>
        /// 判断当前用户是否有权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        protected Task<bool> IsAuthorizedAsync(string permissionName)
        {
            return GetRequiredService<IPermissionManager>().IsAuthorizedAsync(permissionName);
        }

        /// <summary>
        /// 是否已经登入。
        /// </summary>
        protected bool IsAuthenticated => User.Identity.IsAuthenticated;

        private int _userId = -1;
        /// <summary>
        /// 当前用户ID。
        /// </summary>
        protected int UserId
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
        protected string UserName => _userName ?? (_userName = User.GetUserName());
        #endregion

        #region storages
        /// <summary>
        /// 导出Excel。
        /// </summary>
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="models">模型实例列表。</param>
        /// <param name="fileName">导出文件名称。</param>
        /// <returns>返回试图结果。</returns>
        protected IActionResult Excel<TModel>(IEnumerable<TModel> models, string fileName = null)
            where TModel : class, new()
        {
            if (fileName == null)
                fileName = Guid.NewGuid().ToString("N");
            return GetRequiredService<IExcelManager>().Export(models, fileName);
        }

        /// <summary>
        /// 导出Excel。
        /// </summary>
        /// <param name="models">模型实例列表。</param>
        /// <param name="fileName">导出文件名称。</param>
        /// <param name="sheetName">工作表名称。</param>
        /// <returns>返回试图结果。</returns>
        protected IActionResult Excel(DataTable models, string fileName = null, string sheetName = "sheet1")
        {
            if (fileName == null)
                fileName = Guid.NewGuid().ToString("N");
            return GetRequiredService<IExcelManager>().Export(models, fileName, sheetName);
        }

        /// <summary>
        /// 返回上传/下载结果。
        /// </summary>
        /// <returns>返回JSON对象。</returns>
        /// <param name="result">上传/下载结果。</param>
        protected IActionResult Json(MediaResult result)
        {
            if (result.Succeeded)
                return Success(new { result.Url });
            return Error(result.Message);
        }
        #endregion

        #region email
        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="resourceKey">资源键：<paramref name="resourceKey"/>_{Title}，<paramref name="resourceKey"/>_{Content}。</param>
        /// <param name="replacement">替换对象，使用匿名类型实例。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        protected bool SendEmail(UserBase user, string resourceKey, object replacement = null, Action<Email> action = null) =>
            GetRequiredService<IMessageManager>().SendEmail(user, resourceKey, replacement, GetType(), action);

        /// <summary>
        /// 发送电子邮件。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="resourceKey">资源键：<paramref name="resourceKey"/>_{Title}，<paramref name="resourceKey"/>_{Content}。</param>
        /// <param name="replacement">替换对象，使用匿名类型实例。</param>
        /// <param name="action">实例化方法。</param>
        /// <returns>返回发送结果。</returns>
        protected Task<bool> SendEmailAsync(UserBase user, string resourceKey, object replacement = null, Action<Email> action = null) =>
            GetRequiredService<IMessageManager>().SendEmailAsync(user, resourceKey, replacement, GetType(), action);
        #endregion
    }
}