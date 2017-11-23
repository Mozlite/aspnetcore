using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mozlite.Extensions;
using Mozlite.Mvc.Messages;

namespace Mozlite.Mvc
{
    /// <summary>
    /// 控制器基类。
    /// </summary>
    public abstract class ControllerBase : Controller
    {
        #region commons
        private int _pageIndex = -1;
        /// <summary>
        /// 当前页码。
        /// </summary>
        protected virtual int PageIndex
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
            if (!Request.Cookies.TryGetValue(key, out var value))
                return false;
            code = Verifiers.Verifiers.Hashed(code);
            return string.Equals(value, code, StringComparison.OrdinalIgnoreCase);
        }
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
            return View(BsType.Warning, message, model, action);
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
            return View(BsType.Info, message, model, action);
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
            return View(BsType.Danger, message, model, action);
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
            return View(BsType.Success, message, model, action);
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
                return View(BsType.Success, result.ToString(args), model);
            return View(BsType.Danger, result.ToString(args), model);
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
                return View(BsType.Success, result.ToString(args), model, url);
            return View(BsType.Danger, result.ToString(args), model);
        }

        private IActionResult View(BsType type, string message, object model, string url = null)
        {
            // 不是网址
            if (url != null && url.IndexOf('/') == -1)
                url = Url.Action(url, ControllerName, new { area = AreaName });
            ViewBag.BsMessage = new BsMessage(message, type, url);
            return View(model);
        }
        #endregion

        #region jsons
        /// <summary>
        /// 返回消息的JSON对象。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回JSON结果。</returns>
        protected IActionResult Info(string message)
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
        protected IActionResult Warning(string message)
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
        protected IActionResult Error(string message)
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
            return Json(new JsonMesssage(type, message));
        }

        private IActionResult Json<TData>(BsType type, string message, TData data)
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
                return Json(BsType.Success, result.ToString(args));
            return Json(BsType.Danger, result.ToString(args));
        }
        #endregion

        #region pages
        /// <summary>
        /// 设置标题，关键词，和描述。
        /// </summary>
        /// <param name="title">标题。</param>
        /// <param name="description">描述。</param>
        /// <param name="keywords">关键词。</param>
        protected void SetMeta(string title, string description, params object[] keywords)
        {
            ViewBag.Title = title;
            ViewBag.Keywords = keywords.JoinSplit(",");
            ViewBag.Description = description;
        }

        /// <summary>
        /// 设置标题和描述。
        /// </summary>
        /// <param name="title">标题。</param>
        /// <param name="description">描述。</param>
        protected void SetMeta(string title, string description)
        {
            ViewBag.Title = title;
            ViewBag.Description = description;
        }

        /// <summary>
        /// 设置标题。
        /// </summary>
        /// <param name="title">标题。</param>
        protected void SetMeta(string title)
        {
            ViewBag.Title = title;
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
    }
}