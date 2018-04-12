using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 空项目时候的警告信息。
    /// </summary>
    [HtmlTargetElement("moz:warning-tabled")]
    public class WarningTabledTagHelper : TagHelperBase
    {
        /// <summary>
        /// 是否显示。
        /// </summary>
        [HtmlAttributeName("attach")]
        public object Value { get; set; }

        /// <summary>
        /// 样式。
        /// </summary>
        [HtmlAttributeName("class")]
        public string CssClass { get; set; }

        private bool IsAttached()
        {
            if (Value is bool bValue)
                return !bValue;
            if (Value is IEnumerable value)
                return !value.GetEnumerator().MoveNext();
            return Value == null;
        }

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
                builder.MergeAttribute("colspan", "100");
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