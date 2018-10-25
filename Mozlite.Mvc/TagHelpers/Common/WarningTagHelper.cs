using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 警告标签。
    /// </summary>
    [HtmlTargetElement("moz:warning")]
    public class WarningTagHelper : TagHelperBase
    {
        /// <summary>
        /// 是否显示。
        /// </summary>
        [HtmlAttributeName("attach")]
        public object Value { get; set; }

        /// <summary>
        /// 是否显示。
        /// </summary>
        /// <returns>返回判断结果。</returns>
        protected bool IsAttached()
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
                await output.RenderAsync("div", async builder =>
                {
                    builder.AddCssClass("null-warning");
                    builder.InnerHtml.AppendHtml("<i class=\"fa fa-warning\"></i> ");
                    var content = await output.GetChildContentAsync();
                    if (!content.IsEmptyOrWhiteSpace)
                        builder.InnerHtml.AppendHtml(content);
                });
            }
            else
            {
                output.SuppressOutput();
            }
        }
    }
}