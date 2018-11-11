using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
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
        /// 讲当前输出设置为<paramref name="action"/>元素实例。
        /// </summary>
        /// <param name="output">输出实例对象。</param>
        /// <param name="tagName">标签名称。</param>
        /// <param name="action">构建实例对象。</param>
        public static void Render(this TagHelperOutput output, string tagName, Action<TagBuilder> action)
        {
            output.TagName = tagName;
            var builder = new TagBuilder(tagName);
            action(builder);
            output.MergeAttributes(builder);
            output.Content.AppendHtml(builder.InnerHtml);
        }

        /// <summary>
        /// 讲当前输出设置为<paramref name="action"/>元素实例。
        /// </summary>
        /// <param name="output">输出实例对象。</param>
        /// <param name="tagName">标签名称。</param>
        /// <param name="action">构建实例对象。</param>
        public static async Task RenderAsync(this TagHelperOutput output, string tagName, Func<TagBuilder, Task> action)
        {
            output.TagName = tagName;
            var builder = new TagBuilder(tagName);
            await action(builder);
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
            if (!output.Attributes.TryGetAttribute("class", out var attribute))
            {
                output.Attributes.Add("class", string.Join(" ", classNames));
                return;
            }
            var classes = ExtractClassValue(attribute);
            foreach (var className in classNames)
            {
                if (classes.Contains(className))
                    continue;
                classes.Add(className);
            }
            output.SetAttribute("class", string.Join(" ", classes));
        }

        /// <summary>
        /// 移除样式。
        /// </summary>
        /// <param name="output">输出实例对象。</param>
        /// <param name="className">样式表。</param>
        public static void RemoveClass(this TagHelperOutput output, string className)
        {
            if (!output.Attributes.TryGetAttribute("class", out var attribute))
                return;
            var classes = ExtractClassValue(attribute).Where(x => x != className);
            output.SetAttribute("class", string.Join(" ", classes));
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

        private static List<string> ExtractClassValue(TagHelperAttribute classAttribute)
        {
            string extractedClassValue;
            switch (classAttribute.Value)
            {
                case string valueAsString:
                    extractedClassValue = HtmlEncoder.Default.Encode(valueAsString);
                    break;
                case HtmlString valueAsHtmlString:
                    extractedClassValue = valueAsHtmlString.Value;
                    break;
                case IHtmlContent htmlContent:
                    using (var stringWriter = new StringWriter())
                    {
                        htmlContent.WriteTo(stringWriter, HtmlEncoder.Default);
                        extractedClassValue = stringWriter.ToString();
                    }
                    break;
                default:
                    extractedClassValue = HtmlEncoder.Default.Encode(classAttribute.Value?.ToString());
                    break;
            }
            var currentClassValue = extractedClassValue ?? string.Empty;
            return currentClassValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// 添加MarkDown编辑器按钮。
        /// </summary>
        /// <returns>返回当前工具栏标签实例。</returns>
        /// <param name="builder">当前工具栏标签实例。</param>
        /// <param name="key">功能键。</param>
        /// <param name="icon">图标。</param>
        /// <param name="title">标题。</param>
        public static TagBuilder AddSyntax(this TagBuilder builder, string key, string icon, string title)
        {
            return builder.AppendTag("a", a =>
            {
                a.AppendTag("i", x => x.AddCssClass(icon));
                a.MergeAttribute("title", title);
                a.AddCssClass($"mozmd-syntax-{key}");
            });
        }
    }
}