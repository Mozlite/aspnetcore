using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Bootstrap
{
    /// <summary>
    /// 全选框。
    /// </summary>
    [HtmlTargetElement("moz:checkall")]
    public class CheckallTagHelper : CheckboxTagHelper
    {
        /// <summary>
        /// 异步访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);
            output.AddCssClass("moz-checkall");
        }
    }
}