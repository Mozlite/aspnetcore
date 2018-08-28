using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Mozlite.Mvc.TagHelpers
{
    /// <summary>
    /// js脚本标签。
    /// </summary>
    [HtmlTargetElement("moz:jshtml", Attributes = HrefAttributeName)]
    [HtmlTargetElement("moz:jshtml", Attributes = JsonAttributeName)]
    public class MozjsTagHelper : ViewContextableTagHelperBase
    {
        private const string HrefAttributeName = "href";
        private const string JsonAttributeName = "json";

        /// <summary>
        /// 获取JSON对象的地址。
        /// </summary>
        [HtmlAttributeName(HrefAttributeName)]
        public string Url { get; set; }

        /// <summary>
        /// 获取JSON对象的地址。
        /// </summary>
        [HtmlAttributeName(JsonAttributeName)]
        public string Json { get; set; }

        /// <summary>
        /// 下一次获取时间。
        /// </summary>
        [HtmlAttributeName("interval")]
        public int Interval { get; set; }

        /// <summary>
        /// 当前标签名称。
        /// </summary>
        [HtmlAttributeName("tag")]
        public string TagName { get; set; } = "div";

        /// <summary>
        /// 发送的数据。
        /// </summary>
        [HtmlAttributeName("js", DictionaryAttributePrefix = "js-")]
        public IDictionary<string, string> Data { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 异步访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = await output.GetChildContentAsync();
            if (content.IsEmptyOrWhiteSpace)
                return;
            output.TagName = TagName;
            var id = "moz-jshtml-" + GetCounter();
            output.AddCssClass(id);
        }
    }
}