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
        public string FalseIcon { get; set; } = "fa fa-dot-circle-o";

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
            output.TagName = "div";
            if (Status)
            {
                output.AddCssClass(TrueClass);
                output.AppendHtml("i", i => i.AddCssClass(TrueIcon));
                if (!string.IsNullOrEmpty(TrueText))
                    output.AppendHtml("span", span => span.InnerHtml.AppendHtml(TrueText));
            }
            else
            {
                output.AddCssClass(FalseClass);
                output.AppendHtml("i", i => i.AddCssClass(FalseIcon));
                if (!string.IsNullOrEmpty(FalseText))
                    output.AppendHtml("span", span => span.InnerHtml.AppendHtml(FalseText));
            }
        }
    }
}