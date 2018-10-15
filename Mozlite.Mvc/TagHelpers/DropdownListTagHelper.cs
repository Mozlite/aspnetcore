using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mozlite.Mvc.TagHelpers
{
    /// <summary>
    /// 下拉列表标签基类。
    /// </summary>
    public abstract class DropdownListTagHelper : ViewContextableTagHelperBase
    {
        /// <summary>
        /// 模型属性。
        /// </summary>
        [HtmlAttributeName("for")]
        public ModelExpression For { get; set; }

        /// <summary>
        /// 默认显示字符串：如“请选择”。
        /// </summary>
        [HtmlAttributeName("default-text")]
        public string DefaultText { get; set; }

        /// <summary>
        /// 默认值。
        /// </summary>
        [HtmlAttributeName("default-value")]
        public object DefaultValue { get; set; }

        /// <summary>
        /// 值。
        /// </summary>
        [HtmlAttributeName("value")]
        public object Value { get; set; }

        /// <summary>
        /// 异步访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "select";
            var items = Init() ?? await InitAsync() ?? Enumerable.Empty<SelectListItem>();
            if (!string.IsNullOrEmpty(DefaultText))//添加默认选项
                items = new[] { new SelectListItem { Text = DefaultText, Value = DefaultValue?.ToString() } }.Concat(items).ToList();
            if (For != null)
            {
                var htmlGenerator = HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();
                var tagHelper = new SelectTagHelper(htmlGenerator);
                tagHelper.ViewContext = ViewContext;
                tagHelper.For = For;
                tagHelper.Items = items;
                tagHelper.Init(context);
                await tagHelper.ProcessAsync(context, output);
            }
            else
            {
                var value = Value?.ToString();
                if (value != null) output.SetAttribute("value", value);
                foreach (var item in items)
                {
                    output.AppendHtml("option", option =>
                    {
                        if (string.Equals(item.Value, value, StringComparison.OrdinalIgnoreCase))
                            option.MergeAttribute("selected", "selected", true);
                        option.MergeAttribute("value", item.Value);
                        option.InnerHtml.AppendHtml(item.Text);
                    });
                }
            }
        }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected virtual IEnumerable<SelectListItem> Init()
        {
            return null;
        }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected virtual Task<IEnumerable<SelectListItem>> InitAsync()
        {
            return Task.FromResult<IEnumerable<SelectListItem>>(null);
        }
    }
}