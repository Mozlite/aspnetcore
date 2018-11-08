using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers
{
    /// <summary>
    /// MarkDown编辑器。
    /// </summary>
    [HtmlTargetElement("moz:markdown")]
    public class MarkDownTagHelper : ViewContextableTagHelperBase
    {
        /// <summary>
        /// 源码模型属性。
        /// </summary>
        [HtmlAttributeName("md-for")]
        public ModelExpression SourceFor { get; set; }

        /// <summary>
        /// HTML模型属性。
        /// </summary>
        [HtmlAttributeName("html-for")]
        public ModelExpression HtmlFor { get; set; }

        /// <summary>
        /// 源码名称。
        /// </summary>
        [HtmlAttributeName("md-name")]
        public string SourceName { get; set; }

        /// <summary>
        /// HTML名称。
        /// </summary>
        [HtmlAttributeName("html-name")]
        public string HtmlName { get; set; }

        /// <summary>
        /// 值。
        /// </summary>
        [HtmlAttributeName("value")]
        public string Value { get; set; }

        /// <summary>
        /// 上传图片文件地址。
        /// </summary>
        [HtmlAttributeName("upload")]
        public string UploadUrl { get; set; }

        /// <summary>
        /// 初始化当前标签上下文。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        public override void Init(TagHelperContext context)
        {
            if (SourceName == null)
                SourceName = SourceFor?.Name;
            if (HtmlName == null)
                HtmlName = HtmlFor?.Name;
            if (!string.IsNullOrEmpty(Value) || SourceFor == null)
                return;
            Value = SourceFor.Model?.ToString();
        }

        /// <summary>
        /// 异步访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (SourceFor == null && SourceName == null)
            {
                output.SuppressOutput();
                return;
            }
            var actions = await output.GetChildContentAsync();
            output.Render("div", builder =>
            {
                if (UploadUrl != null)
                    builder.MergeAttribute("upload-url", UploadUrl);
                builder.AddCssClass("mozmd-editor");
                builder.AppendTag("div", toolbar =>
                {
                    toolbar.AddCssClass("mozmd-toolbar");
                    toolbar.AppendTag("div", left =>
                    {
                        left.AddCssClass("mozmd-left");
                        if(!actions.IsEmptyOrWhiteSpace)
                            left.InnerHtml.AppendHtml(actions.GetContent().Trim());
                        left.InnerHtml.AppendHtml("<a class=\"mozmd-mode-preview\" title=\"预览\"><i class=\"fa fa-eye\"></i></a>");
                    });
                    toolbar.AppendTag("div", right =>
                    {
                        right.AddCssClass("mozmd-right");
                        right.InnerHtml.AppendHtml("<a class=\"mozmd-fullscreen\" title=\"全屏显示\"><i class=\"fa fa-window-maximize\"></i></a>");
                    });
                });
                builder.AppendTag("div", source =>
                {
                    source.AddCssClass("mozmd-source customScrollBar overlay");
                    source.MergeAttribute("contenteditable", "plaintext-only");
                    source.InnerHtml.Append(Value);
                });
                builder.AppendTag("div", source =>
                {
                    source.AddCssClass("mozmd-preview customScrollBar overlay");
                });
                builder.AppendTag("textarea", x =>
                {
                    x.MergeAttribute("name", SourceName);
                    x.AddCssClass("hide mozmd-source-value");
                });
                if (!string.IsNullOrEmpty(HtmlName))
                {
                    builder.AppendTag("textarea", x =>
                    {
                        x.MergeAttribute("name", HtmlName);
                        x.AddCssClass("hide mozmd-html-value");
                    });
                }
            });
        }
    }
}
