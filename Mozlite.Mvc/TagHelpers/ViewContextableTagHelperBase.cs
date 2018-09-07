using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozlite.Mvc.TagHelpers
{
    /// <summary>
    /// 可访问<see cref="ViewContext"/>实例的标记。
    /// </summary>
    public abstract class ViewContextableTagHelperBase : TagHelperBase
    {
        /// <summary>
        /// 试图上下文。
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// HTTP上下文实例。
        /// </summary>
        protected HttpContext HttpContext => ViewContext.HttpContext;

        /// <summary>
        /// 获取当前文档的计数器。
        /// </summary>
        /// <returns>返回计数器值。</returns>
        protected int GetCounter()
        {
            var current = (HttpContext.Items["taghelper:counter"] as int?) ?? 0;
            current++;
            HttpContext.Items["taghelper:counter"] = current;
            return current;
        }

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
    }
}