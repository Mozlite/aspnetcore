using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozlite.Mvc.Logging;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API基类。
    /// </summary>
    public abstract class ApiControllerBase : Controller
    {
        #region result
        /// <summary>
        /// 成功。
        /// </summary>
        protected ApiResult Succeeded() => ApiResult.Succeeded;

        /// <summary>
        /// 失败。
        /// </summary>
        protected ApiResult Failured() => ApiResult.Failured;

        /// <summary>
        /// 未知错误。
        /// </summary>
        protected ApiResult UnknownError() => ApiResult.UnknownError;

        /// <summary>
        /// 返回错误代码。
        /// </summary>
        /// <param name="code">错误码。</param>
        /// <returns>返回API结果。</returns>
        protected ApiResult Error(ErrorCode code) => new ApiResult(code);

        /// <summary>
        /// 返回错误代码。
        /// </summary>
        /// <param name="code">错误码。</param>
        /// <param name="msg">错误信息。</param>
        /// <returns>返回API结果。</returns>
        protected ApiResult Error(ErrorCode code, string msg) => new ApiResult(code, msg);

        /// <summary>
        /// 空参数。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>返回API结果。</returns>
        protected ApiResult NullParameter(string name) => new ApiResult(ErrorCode.NullParameter).Format(name);

        /// <summary>
        /// 参数错误。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>返回API结果。</returns>
        protected ApiResult InvalidParameter(string name) => new ApiResult(ErrorCode.InvalidParameter).Format(name);

        /// <summary>
        /// 数据实例对象。
        /// </summary>
        /// <param name="data">数据实例对象。</param>
        /// <returns>返回数据实例对象。</returns>
        protected ApiDataResult Data(object data) => new ApiDataResult(data);
        #endregion

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
        protected virtual int PageIndex
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

        private Application _application;
        /// <summary>
        /// 当前应用程序实例。
        /// </summary>
        protected Application Application => _application ?? (_application = HttpContext.Items[typeof(Application)] as Application);
    }
}