using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Bootstrap
{
    /// <summary>
    /// 复选框。
    /// </summary>
    [HtmlTargetElement("moz:checkbox")]
    public class CheckboxTagHelper : ViewContextableTagHelperBase
    {
        /// <summary>
        /// 名称。
        /// </summary>
        [HtmlAttributeName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 当前值。
        /// </summary>
        [HtmlAttributeName("value")]
        public string Value { get; set; }

        /// <summary>
        /// 设置属性模型。
        /// </summary>
        [HtmlAttributeName("for")]
        public ModelExpression For { get; set; }

        /// <summary>
        /// 每项样式类型。
        /// </summary>
        [HtmlAttributeName("iclass")]
        public string ItemClass { get; set; }

        /// <summary>
        /// 每项选中样式类型。
        /// </summary>
        [HtmlAttributeName("istyle")]
        public CheckedStyle CheckedStyle { get; set; } = CheckedStyle.Check;

        /// <summary>
        /// 是否选中。
        /// </summary>
        [HtmlAttributeName("checked")]
        public bool IsChecked { get; set; }

        /// <summary>
        /// 初始化当前标签上下文。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        public override void Init(TagHelperContext context)
        {
            if (string.IsNullOrEmpty(Name) && For != null)
            {
                Name = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
                IsChecked = Convert.ToBoolean(For.Model);
            }
        }

        /// <summary>
        /// 异步访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await output.RenderAsync("div", async wrapper =>
            {
                wrapper.AddCssClass("moz-checkbox");
                if (ItemClass != null)
                    wrapper.AddCssClass(ItemClass);
                if (IsChecked)
                    wrapper.AddCssClass("checked");
                wrapper.AddCssClass("checked-style-" + CheckedStyle.ToString().ToLower());

                var input = new TagBuilder("input");
                input.MergeAttribute("type", "checkbox");
                if (!string.IsNullOrEmpty(Name))
                    input.MergeAttribute("name", Name);
                input.MergeAttribute("value", Value ?? "true");
                if (IsChecked)
                    input.MergeAttribute("checked", "checked");
                input.TagRenderMode = TagRenderMode.SelfClosing;
                wrapper.InnerHtml.AppendHtml(input);

                var label = new TagBuilder("label");
                label.AddCssClass("box-wrapper");
                label.InnerHtml.AppendHtml("<div class=\"box-checked\"></div>");
                wrapper.InnerHtml.AppendHtml(label);

                var text = await output.GetChildContentAsync();
                if (!text.IsEmptyOrWhiteSpace)
                {
                    var span = new TagBuilder("span");
                    span.InnerHtml.AppendHtml(text);
                    wrapper.InnerHtml.AppendHtml(span);
                }
            });
        }
    }
}