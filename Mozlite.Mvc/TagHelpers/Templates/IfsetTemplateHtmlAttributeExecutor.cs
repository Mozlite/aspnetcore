using System.Collections.Generic;
using System.Text;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// Ifset关键词。
    /// </summary>
    /// <example>
    /// 格式
    ///     moz:ifset-class="{{(name=null):'true-class':'false-class'}}"
    ///     moz:ifset-class="{{(name=null):'true-class'}}"
    /// </example>
    public class IfsetTemplateHtmlAttributeExecutor : ITemplateHtmlAttributeExecutor
    {
        /// <summary>
        /// 关键词。
        /// </summary>
        public string Keyword => "ifset";

        /// <summary>
        /// 通过当前对象执行属性所得到的字符串。
        /// </summary>
        /// <param name="attribute">属性实例对象。</param>
        /// <param name="result">当前原有的属性列表。</param>
        /// <returns>返回脚本代码</returns>
        public string Execute(TemplateHtmlCodeAttribute attribute, Dictionary<string, string> result)
        {
            var sb = new StringBuilder();
            if (result.TryGetValue(attribute.AttributeName, out var value))
            {
                result.Remove(attribute.AttributeName);
            }
            sb.Append("if(")
                .Append(attribute.Value)
                .Append("){")
                .SetJsAttribute(attribute.AttributeName, attribute[0]);
            if (attribute.Count == 2)
                sb.Append("}else{").SetJsAttribute(attribute.AttributeName, attribute[1]);
            else if (!string.IsNullOrWhiteSpace(value))
                sb.Append("}else{").SetJsAttribute(attribute.AttributeName, value);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 通过当前对象执行属性所得到的字符串。
        /// </summary>
        /// <param name="attribute">属性实例对象。</param>
        /// <param name="result">当前原有的属性列表。</param>
        /// <param name="instance">当前实例对象。</param>
        public void Execute(TemplateHtmlCodeAttribute attribute, Dictionary<string, string> result, object instance)
        {
            var value = attribute.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                var isTrue = (bool)TemplateExpression.Execute(value, instance);
                if (isTrue)
                    result[attribute.AttributeName] = TemplateExpression.Execute(attribute[0], instance).ToString();
                else if (attribute.Count == 2)
                    result[attribute.AttributeName] = TemplateExpression.Execute(attribute[0], instance).ToString();
            }
        }
    }
}