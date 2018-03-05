using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 是否添加当前按钮样式。
    /// </summary>
    [HtmlTargetElement("*", Attributes = AttributeName)]
    public class ActiveTagHelper : ViewContextableTagHelperBase
    {
        private const string AttributeName = ".active";
        private const string CurrentAttributeName = ".current";

        [HtmlAttributeName(AttributeName)]
        public string ActiveValue { get; set; }

        [HtmlAttributeName(CurrentAttributeName)]
        public string CurrentValue { get; set; }

        [HtmlAttributeName(".class")]
        public string ActiveClass { get; set; } = "active";

        /// <summary>
        /// 访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (CurrentValue == null)
                CurrentValue = ViewContext.ViewBag.Current as string;
            if (string.Equals(ActiveValue, CurrentValue, StringComparison.OrdinalIgnoreCase))
            {
                output.AddCssClass(ActiveClass);
            }
        }
    }
}