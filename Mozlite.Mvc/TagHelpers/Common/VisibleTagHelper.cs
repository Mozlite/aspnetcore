using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 显示标签。
    /// </summary>
    [HtmlTargetElement("*", Attributes = AttributeName)]
    public class VisibleTagHelper : TagHelperBase
    {
        private const string AttributeName = ".visible";
        /// <summary>
        /// 是否显示。
        /// </summary>
        [HtmlAttributeName(AttributeName)]
        public bool IsVisible { get; set; }

        /// <summary>
        /// 访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsVisible)
                return;
            output.SuppressOutput();
        }
    }
}