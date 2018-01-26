using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Extensions.Storages.Caching
{
    /// <summary>
    /// 缓存标签。
    /// </summary>
    [HtmlTargetElement("moz:cache", Attributes = KeyAttributeName)]
    public class CacheTagHelper : TagHelper
    {
        private const string KeyAttributeName = "key";
        private const string ExpiredAttributeName = "expired";

        private readonly IStorageCache _cache;
        /// <summary>
        /// 初始化类<see cref="CacheTagHelper"/>。
        /// </summary>
        /// <param name="cache">缓存实例对象。</param>
        protected CacheTagHelper(IStorageCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 试图上下文。
        /// </summary>
        [ViewContext]
        [HtmlAttributeNotBound]
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// HTTP上下文实例。
        /// </summary>
        protected HttpContext HttpContext => ViewContext.HttpContext;

        /// <summary>
        /// 元素Id，必须确保一个页面中唯一存在。
        /// </summary>
        [HtmlAttributeName(KeyAttributeName)]
        public string Key { get; set; }

        /// <summary>
        /// 过期时间长度，从当前时间开始。
        /// </summary>
        [HtmlAttributeName(ExpiredAttributeName)]
        public TimeSpan? Expired { get; set; }

        /// <summary>
        /// 当前元素节点名称。
        /// </summary>
        [HtmlAttributeName("tag")]
        public string TagName { get; set; } = "div";

        /// <summary>
        /// 呈现标签。
        /// </summary>
        /// <param name="context">当前标签上下文。</param>
        /// <param name="output">标签输出实例对象。</param>
        /// <returns>返回当前标签任务。</returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagName;
            var viewFile = new FileStorageCacheDependency(Path.Combine(Directory.GetCurrentDirectory(), ViewContext.ExecutingFilePath));
            output.Content.AppendHtml(await _cache.GetOrCreateAsync(ViewContext.ExecutingFilePath + ":" + Key, viewFile, async ctx =>
            {
                //一天。
                if (Expired != null)
                    ctx.SetAbsoluteExpiredDate(Expired.Value);
                return await output.GetChildContentAsync();
            }));
        }
    }
}