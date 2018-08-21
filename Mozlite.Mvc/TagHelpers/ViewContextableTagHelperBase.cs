using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

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
    }
}