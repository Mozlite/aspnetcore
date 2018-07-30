using System;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 语句执行解析接口。
    /// </summary>
    public interface ITemplateSyntaxExecutor : IServices
    {
        /// <summary>
        /// 关键词。
        /// </summary>
        string Keyword { get; }

        /// <summary>
        /// 通过当前对象执行语法所得到的字符串。
        /// </summary>
        /// <param name="element">属性实例对象。</param>
        /// <param name="executor">解析器接口。</param>
        /// <param name="instance">当前实例对象。</param>
        /// <param name="func">获取当前对象属性值的方法。</param>
        /// <returns>返回脚本代码</returns>
        string End(TemplateSyntaxElement element, ITemplateExecutor executor, object instance, Func<object, string, object> func);

        /// <summary>
        /// 通过当前对象执行语法所得到的字符串。
        /// </summary>
        /// <param name="element">属性实例对象。</param>
        /// <param name="executor">解析器接口。</param>
        /// <returns>返回脚本代码</returns>
        string Begin(TemplateSyntaxElement element, ITemplateExecutor executor);

        /// <summary>
        /// 通过当前对象执行语法所得到的字符串。
        /// </summary>
        /// <param name="element">属性实例对象。</param>
        /// <param name="executor">解析器接口。</param>
        /// <param name="instance">当前实例对象。</param>
        /// <param name="func">获取当前对象属性值的方法。</param>
        /// <returns>返回脚本代码</returns>
        string Begin(TemplateSyntaxElement element, ITemplateExecutor executor, object instance, Func<object, string, object> func);

        /// <summary>
        /// 通过当前对象执行语法所得到的字符串。
        /// </summary>
        /// <param name="element">属性实例对象。</param>
        /// <param name="executor">解析器接口。</param>
        /// <returns>返回脚本代码</returns>
        string End(TemplateSyntaxElement element, ITemplateExecutor executor);
    }
}