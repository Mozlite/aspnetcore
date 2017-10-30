using System;
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
    }
}