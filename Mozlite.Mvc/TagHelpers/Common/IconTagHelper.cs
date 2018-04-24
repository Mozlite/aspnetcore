using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 图标标签。
    /// </summary>
    [HtmlTargetElement("moz:icon")]
    public class IconTagHelper : TagHelperBase
    {
        /// <summary>
        /// 样式或者图片地址。
        /// </summary>
        [HtmlAttributeName("src")]
        public string Src { get; set; }

        /// <summary>
        /// 访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(Src))
            {
                output.SuppressOutput();
                return;
            }
            var builder = new TagBuilder("div");
            if (Src.IndexOf('.') == -1)
            {
                output.TagName = "i";
                builder.AddCssClass(Src);
            }
            else
            {
                output.TagName = "img";
                builder.MergeAttribute("src", Src);
            }
            output.MergeAttributes(builder);
        }
    }
}
