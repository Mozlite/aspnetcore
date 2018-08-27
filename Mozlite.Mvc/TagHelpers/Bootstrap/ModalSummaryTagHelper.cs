using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Mozlite.Mvc.TagHelpers.Bootstrap
{
    /// <summary>
    /// 显示弹窗模板错误信息栏。
    /// </summary>
    [HtmlTargetElement("moz:modal-summary")]
    public class ModalSummaryTagHelper : TagHelperBase
    {
        /// <summary>
        /// 图标样式。
        /// </summary>
        [HtmlAttributeName("icon")]
        public string IconClass { get; set; } = "fa fa-warning";

        /// <summary>
        /// 异步访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await output.RenderAsync("div", async builder =>
             {
                 builder.AddCssClass("modal-summary");
                 if (!string.IsNullOrEmpty(IconClass))
                     builder.AppendTag("i", tb => tb.AddCssClass(IconClass));
                 var content = await output.GetChildContentAsync();
                 builder.AppendTag("span", tb =>
                 {
                     tb.AddCssClass("modal-summary-text");
                     if (!content.IsEmptyOrWhiteSpace)
                     {
                         builder.MergeAttribute("style", "display:block", true);
                         tb.InnerHtml.AppendHtml(content);
                     }
                 });
             });
        }
    }
}