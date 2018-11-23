using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Mvc.Properties;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Mvc.TagHelpers
{
    /// <summary>
    /// MarkDown编辑器。
    /// </summary>
    [HtmlTargetElement("moz:markdown")]
    [HtmlTargetElement("moz:markdown", Attributes = UploadValuesPrefix + "*")]
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

        private const string UploadValuesPrefix = "upload-";
        private const string UploadValuesDictionaryName = "upload-data";
        private IDictionary<string, object> _uploadData;
        /// <summary>
        /// 样式列表。
        /// </summary>
        [HtmlAttributeName(UploadValuesDictionaryName, DictionaryAttributePrefix = UploadValuesPrefix)]
        public IDictionary<string, object> UploadData
        {
            get
            {
                if (_uploadData == null)
                {
                    _uploadData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                }

                return _uploadData;
            }
            set => _uploadData = value;
        }

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
                {
                    builder.MergeAttribute("js-upload-url", UploadUrl);
                    foreach (var uploadData in UploadData)
                    {
                        builder.MergeAttribute("js-upload-data-" + uploadData.Key.ToLower(), uploadData.Value?.ToString());
                    }
                }
                builder.AddCssClass("mozmd-editor");
                builder.AppendTag("div", toolbar =>
                {
                    toolbar.AddCssClass("mozmd-toolbar");
                    toolbar.AppendTag("div", left =>
                    {
                        left.AddCssClass("mozmd-left");
                        if (!actions.IsEmptyOrWhiteSpace)
                            left.InnerHtml.AppendHtml(actions.GetContent().Trim());
                        ProcessToolbar(left);
                        left.AppendTag("a", a =>
                        {
                            a.AppendTag("i", x => x.AddCssClass("fa fa-eye"));
                            a.MergeAttribute("title", Resources.Mozmd_ModePreview);
                            a.AddCssClass("mozmd-mode-preview");
                        });
                    });
                    toolbar.AppendTag("div", right =>
                    {
                        right.AddCssClass("mozmd-right");
                        ProcessRightToolbar(right);
                        right.AppendTag("a", a =>
                        {
                            a.AppendTag("i", x => x.AddCssClass("fa fa-window-maximize"));
                            a.MergeAttribute("title", Resources.Mozmd_FullScreen);
                            a.AddCssClass("mozmd-fullscreen");
                        });
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
                    source.AddCssClass("mozmd-preview customScrollBar overlay txt");
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

        /// <summary>
        /// 添加工具栏按钮。
        /// </summary>
        /// <param name="builder">Html内容构建实例。</param>
        protected virtual void ProcessToolbar(TagBuilder builder)
        {
            builder.AddSyntax("header", "fa fa-header", Resources.Mozmd_Syntax_Header)
                   .AddSyntax("bold", "fa fa-bold", Resources.Mozmd_Syntax_Bold)
                   .AddSyntax("italic", "fa fa-italic", Resources.Mozmd_Syntax_Italic)
                   .AddSyntax("ul", "fa fa-list-ul", Resources.Mozmd_Syntax_Ul)
                   .AddSyntax("ol", "fa fa-list-ol", Resources.Mozmd_Syntax_Ol)
                   .AddSyntax("link", "fa fa-link", Resources.Mozmd_Syntax_Link)
                   .AddSyntax("image", "fa fa-image", Resources.Mozmd_Syntax_Image)
                   .AddSyntax("quote", "fa fa-quote-right", Resources.Mozmd_Syntax_Quote)
                   .AddSyntax("code", "fa fa-code", Resources.Mozmd_Syntax_Code);
        }

        /// <summary>
        /// 添加工具栏右边按钮。
        /// </summary>
        /// <param name="builder">Html内容构建实例。</param>
        protected virtual void ProcessRightToolbar(TagBuilder builder)
        {

        }
    }
}
