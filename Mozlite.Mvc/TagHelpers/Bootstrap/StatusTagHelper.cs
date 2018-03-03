using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Bootstrap
{
    /// <summary>
    /// 状态显示标签。
    /// </summary>
    [HtmlTargetElement("div", Attributes = AttributeName)]
    [HtmlTargetElement("span", Attributes = AttributeName)]
    public class StatusTagHelper : TagHelperBase
    {
        private const string AttributeName = ".status";
        /// <summary>
        /// 状态。
        /// </summary>
        [HtmlAttributeName(AttributeName)]
        public bool Status { get; set; }

        [HtmlAttributeName(".true")]
        public string TrueIcon { get; set; } = "fa fa-check-square-o";

        [HtmlAttributeName(".false")]
        public string FalseIcon { get; set; } = "fa dot-circle-o";

        [HtmlAttributeName(".true-text")]
        public string TrueText { get; set; }

        [HtmlAttributeName(".false-text")]
        public string FalseText { get; set; }

        [HtmlAttributeName(".true-class")]
        public string TrueClass { get; set; } = "text-success";

        [HtmlAttributeName(".false-class")]
        public string FalseClass { get; set; } = "text-danger";

        /// <summary>
        /// 访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var builder = new TagBuilder("div");
            var icon = new TagBuilder("i");
            if (Status)
            {
                if (!string.IsNullOrEmpty(TrueText))
                    output.Content.AppendHtml($"<span>{TrueText}</span>");
                builder.AddCssClass(TrueClass);
                icon.AddCssClass(TrueIcon);
            }
            else
            {
                if (!string.IsNullOrEmpty(FalseText))
                    output.Content.AppendHtml($"<span>{FalseText}</span>");
                builder.AddCssClass(FalseClass);
                icon.AddCssClass(FalseClass);
            }
            output.PreContent.AppendHtml(icon);
            output.MergeAttributes(builder);
        }
    }
}