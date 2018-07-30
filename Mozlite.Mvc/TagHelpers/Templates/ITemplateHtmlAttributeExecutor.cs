using System.Collections.Generic;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 属性执行接口，一般包含"moz:"开头的属性。
    /// </summary>
    public interface ITemplateHtmlAttributeExecutor : IServices
    {
        /// <summary>
        /// 关键词。
        /// </summary>
        string Keyword { get; }

        /// <summary>
        /// 通过当前对象执行属性所得到的字符串。
        /// </summary>
        /// <param name="attribute">属性实例对象。</param>
        /// <param name="result">当前原有的属性列表。</param>
        /// <returns>返回脚本代码</returns>
        string Execute(TemplateHtmlCodeAttribute attribute, Dictionary<string, string> result);

        /// <summary>
        /// 通过当前对象执行属性所得到的字符串。
        /// </summary>
        /// <param name="attribute">属性实例对象。</param>
        /// <param name="result">当前原有的属性列表。</param>
        /// <param name="instance">当前实例对象。</param>
        void Execute(TemplateHtmlCodeAttribute attribute, Dictionary<string, string> result, object instance);
    }
}