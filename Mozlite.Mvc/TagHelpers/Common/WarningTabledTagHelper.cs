using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 空项目时候的警告信息。
    /// </summary>
    [HtmlTargetElement("moz:warning-tabled")]
    public class WarningTabledTagHelper : WarningTagHelper
    {
        /// <summary>
        /// 列数。
        /// </summary>
        [HtmlAttributeName("cols")]
        public int Cols { get; set; } = 100;

        /// <summary>
        /// 异步访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (IsAttached())
            {
                var content = await output.GetChildContentAsync();
                output.TagName = "tr";
                var builder = new TagBuilder("td");
                if (CssClass != null)
                    builder.AddCssClass(CssClass);
                if (Cols > 1)
                    builder.MergeAttribute("colspan", Cols.ToString());
                builder.InnerHtml.AppendHtml("<i class=\"fa fa-warning\"></i> ");
                if (!content.IsEmptyOrWhiteSpace)
                    builder.InnerHtml.AppendHtml(content);
                output.Content.AppendHtml(builder);
            }
            else
            {
                output.SuppressOutput();
            }
        }
    }
}