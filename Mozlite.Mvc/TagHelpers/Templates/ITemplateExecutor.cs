using System;
using System.Collections.Generic;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 模板语法解析器接口。
    /// </summary>
    public interface ITemplateExecutor : ISingletonService
    {
        /// <summary>
        /// 尝试获取语法解析器。
        /// </summary>
        /// <param name="element">元素实例对象。</param>
        /// <param name="executor">返回的解析器。</param>
        /// <returns>返回获取结果。</returns>
        bool TryGetExecutor(TemplateSyntaxElement element, out ITemplateSyntaxExecutor executor);

        /// <summary>
        /// 通过当前对象执行属性所得到的字符串。
        /// </summary>
        /// <param name="attributes">属性实例对象列表。</param>
        /// <param name="result">当前原有的属性列表。</param>
        /// <returns>返回脚本代码</returns>
        string Execute(IEnumerable<TemplateHtmlCodeAttribute> attributes, Dictionary<string, string> result);

        /// <summary>
        /// 通过当前对象执行属性所得到的字符串。
        /// </summary>
        /// <param name="attributes">属性实例对象列表。</param>
        /// <param name="result">当前原有的属性列表。</param>
        /// <param name="instance">当前实例对象。</param>
        /// <param name="func">获取当前对象属性值的方法。</param>
        void Execute(IEnumerable<TemplateHtmlCodeAttribute> attributes, Dictionary<string, string> result, object instance, Func<object, string, object> func);
    }
}