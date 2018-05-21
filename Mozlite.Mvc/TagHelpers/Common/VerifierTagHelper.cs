using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 验证码图片。
    /// </summary>
    [HtmlTargetElement("moz:verifier")]
    public class VerifierTagHelper : TagHelperBase
    {
        /// <summary>
        /// 字符个数。
        /// </summary>
        [HtmlAttributeName("length")]
        public int Length { get; set; }

        /// <summary>
        /// 字体大小。
        /// </summary>
        [HtmlAttributeName("size")]
        public int FontSize { get; set; }

        /// <summary>
        /// 高度。
        /// </summary>
        [HtmlAttributeName("height")]
        public int Height { get; set; }

        /// <summary>
        /// 验证唯一键。
        /// </summary>
        [HtmlAttributeName("key")]
        public string Key { get; set; }

        /// <summary>
        /// 访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Render("img", builder =>
            {
                var url = $"/{Key}-vcode.png?n={Length}&s={FontSize}&h={Height}";
                builder.MergeAttribute("src", url);
                builder.MergeAttribute("onclick", $"this.src='{url}&_' + (+new Date());");
                builder.MergeAttribute("title", "点击刷新验证码");
            });
        }
    }
}