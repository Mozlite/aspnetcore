using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers
{
    /// <summary>
    /// 扩展类。
    /// </summary>
    public static class TagHelperExtensions
    {
        /// <summary>
        /// 添加子元素。
        /// </summary>
        /// <param name="builder">当前标签构建实例。</param>
        /// <param name="tagName">子元素名称。</param>
        /// <param name="action">子元素配置方法。</param>
        /// <returns>返回当前标签实例。</returns>
        public static TagBuilder AppendTag(this TagBuilder builder, string tagName, Action<TagBuilder> action)
        {
            var tag = new TagBuilder(tagName);
            action(tag);
            builder.InnerHtml.AppendHtml(tag);
            return builder;
        }

        /// <summary>
        /// 讲当前输出设置为<paramref name="builder"/>元素实例。
        /// </summary>
        /// <param name="output">输出实例对象。</param>
        /// <param name="builder">构建实例对象。</param>
        public static void SetTag(this TagHelperOutput output, TagBuilder builder)
        {
            output.TagName = builder.TagName;
            output.MergeAttributes(builder);
            output.Content.AppendHtml(builder.InnerHtml);
        }

        /// <summary>
        /// 附加HTML内容。
        /// </summary>
        /// <param name="output">当前输出实例。</param>
        /// <param name="htmlContent">HTML内容。</param>
        public static void AppendHtml(this TagHelperOutput output, IHtmlContent htmlContent)
        {
            output.Content.AppendHtml(htmlContent);
        }

        /// <summary>
        /// 附加HTML内容。
        /// </summary>
        /// <param name="output">当前输出实例。</param>
        /// <param name="encoded">HTML内容。</param>
        public static void AppendHtml(this TagHelperOutput output, string encoded)
        {
            output.Content.AppendHtml(encoded);
        }

        /// <summary>
        /// 附加HTML标签。
        /// </summary>
        /// <param name="output">当前输出实例。</param>
        /// <param name="tagName">标签名称。</param>
        /// <param name="action">HTML标签实例化方法。</param>
        public static void AppendHtml(this TagHelperOutput output, string tagName, Action<TagBuilder> action)
        {
            var builder = new TagBuilder(tagName);
            action(builder);
            output.Content.AppendHtml(builder);
        }

        /// <summary>
        /// 获取属性值。
        /// </summary>
        /// <param name="output">当前输出实例。</param>
        /// <param name="attributeName">属性名称。</param>
        /// <returns>返回当前属性值。</returns>
        public static string GetAttribute(this TagHelperOutput output, string attributeName)
        {
            if (output.Attributes.TryGetAttribute(attributeName, out var attribute))
                return attribute.Value?.ToString();
            return null;
        }

        /// <summary>
        /// 添加样式。
        /// </summary>
        /// <param name="output">输出实例对象。</param>
        /// <param name="classNames">样式列表。</param>
        public static void AddCssClass(this TagHelperOutput output, params string[] classNames)
        {
            var @class = output.GetAttribute("class")?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (@class == null) @class = new List<string>();
            foreach (var className in classNames)
            {
                if (@class.Contains(className))
                    continue;
                @class.Add(className);
            }
            output.SetAttribute("class", string.Join(" ", @class));
        }

        /// <summary>
        /// 添加样式。
        /// </summary>
        /// <param name="output">输出实例对象。</param>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值。</param>
        public static void SetAttribute(this TagHelperOutput output, string name, string value)
        {
            output.Attributes.SetAttribute(new TagHelperAttribute(name, value));
        }
    }
}