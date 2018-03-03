using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Data;
using Mozlite.Extensions.Security;
using Mozlite.Mvc.Messages;

namespace Mozlite.Mvc
{
    /// <summary>
    /// 页面模型基类。
    /// </summary>
    public abstract class ModelBase : PageModel
    {
        #region common
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
                    _logger = HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                        .CreateLogger(GetType());
                }
                return _logger;
            }
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
                    if (RouteData.Values.TryGetValue("page", out var pobject))
                        page = pobject.ToString();
                    else
                        page = Request.Query["page"];
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
            code = Verifiers.Verifiers.Hashed(code);
            return string.Equals(value, code, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region users
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
        #endregion
    }

    /// <summary>
    /// 页面模型基类。
    /// </summary>
    /// <typeparam name="TModel">模型实例类型。</typeparam>
    public abstract class ModelBase<TModel> : ModelBase
    {
        /// <summary>
        /// 当前模型实例。
        /// </summary>
        [BindProperty]
        public TModel Model { get; set; }
    }
}