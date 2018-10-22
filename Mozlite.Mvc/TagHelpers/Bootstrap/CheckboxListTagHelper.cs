using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Bootstrap
{
    /// <summary>
    /// 复选框列表标签。
    /// </summary>
    public abstract class CheckboxListTagHelper : ViewContextableTagHelperBase
    {
        /// <summary>
        /// 名称。
        /// </summary>
        [HtmlAttributeName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 以“,”分割值。
        /// </summary>
        [HtmlAttributeName("value")]
        public string Value { get; set; }

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
        /// 设置属性模型。
        /// </summary>
        [HtmlAttributeName("for")]
        public ModelExpression For { get; set; }

        /// <summary>
        /// 初始化当前标签上下文。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        public override void Init(TagHelperContext context)
        {
            if (string.IsNullOrEmpty(Name) && For != null)
            {
                Name = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
                if (Value == null)
                {
                    if (For.Model is string str)
                        Value = str;
                    else if (For.Model is IEnumerable array)
                        Value = array.Join();
                    else
                        Value = For.Model?.ToString();
                }
            }
        }

        /// <summary>
        /// 访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Value != null)
                Value = $",{Value},";
            var items = new Dictionary<string, string>();
            Init(items);
            foreach (var item in items)
            {
                output.Content.AppendHtml(Create(item.Key, item.Value, IsChecked(item.Value)));
            }
            output.Render("div", builder => builder.AddCssClass("moz-checkboxlist"));
        }

        /// <summary>
        /// 判断选中的状态。
        /// </summary>
        /// <param name="current">当前项目值。</param>
        /// <returns>返回判断结果。</returns>
        protected virtual bool IsChecked(string current)
        {
            return Value?.IndexOf($",{current},") >= 0;
        }

        /// <summary>
        /// 附加复选项目列表，文本/值。
        /// </summary>
        /// <param name="items">复选框项目列表实例。</param>
        protected abstract void Init(IDictionary<string, string> items);

        private TagBuilder Create(string text, string value, bool isChecked)
        {
            var wrapper = new TagBuilder("div");
            wrapper.AddCssClass("moz-checkbox");
            if (ItemClass != null)
                wrapper.AddCssClass(ItemClass);
            if (isChecked)
                wrapper.AddCssClass("checked");
            wrapper.AddCssClass("checked-style-" + CheckedStyle.ToString().ToLower());

            var input = new TagBuilder("input");
            input.MergeAttribute("type", "checkbox");
            input.MergeAttribute("name", Name);
            input.MergeAttribute("value", value);
            if (isChecked)
                input.MergeAttribute("checked", "checked");
            input.TagRenderMode = TagRenderMode.SelfClosing;
            wrapper.InnerHtml.AppendHtml(input);

            var label = new TagBuilder("label");
            label.AddCssClass("box-wrapper");
            label.InnerHtml.AppendHtml("<div class=\"box-checked\"></div>");
            wrapper.InnerHtml.AppendHtml(label);

            var span = new TagBuilder("span");
            span.InnerHtml.AppendHtml(text);
            wrapper.InnerHtml.AppendHtml(span);
            return wrapper;
        }
    }
}