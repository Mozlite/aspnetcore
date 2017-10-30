using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Bootstrap
{
    /// <summary>
    /// 显示弹窗模板错误信息栏。
    /// </summary>
    [HtmlTargetElement("moz:model-alert")]
    public class ModalAlertTagHelper : TagHelperBase
    {
        /// <summary>
        /// 访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var builder = new TagBuilder("div");
            builder.AddCssClass("modal-alert");
            builder.AppendTag("span", tb => tb.AddCssClass("fa fa-warning"));
            builder.AppendTag("span", tb => tb.AddCssClass("errmsg"));
            output.SetTag(builder);
        }
    }
}