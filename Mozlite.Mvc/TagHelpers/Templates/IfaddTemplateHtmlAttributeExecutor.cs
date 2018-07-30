using System.Collections.Generic;
using System.Text;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// Ifadd关键词。
    /// </summary>
    /// <example>
    /// 格式
    ///     moz:ifadd-class="{{(name=null):'true-class':'false-class'}}"
    ///     moz:ifadd-class="{{(name=null):'true-class'}}"
    /// </example>
    public class IfaddTemplateHtmlAttributeExecutor : ITemplateHtmlAttributeExecutor
    {
        /// <summary>
        /// 关键词。
        /// </summary>
        public string Keyword => "ifadd";

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
                .MergeJsAttribute(attribute.AttributeName, attribute[0], value)
                .Append("}");
            if (attribute.Count == 2)
                sb.Append("else{")
                    .MergeJsAttribute(attribute.AttributeName, attribute[1], value)
                    .Append("}");
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
            if (result.TryGetValue(attribute.AttributeName, out var attrValue))
                attrValue += " ";
            var value = attribute.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                var isTrue = (bool)TemplateExpression.Execute(value, instance);
                if (isTrue)
                    result[attribute.AttributeName] = attrValue + attribute[0].Trim('\'');
                else if (attribute.Count == 2)
                    result[attribute.AttributeName] = attrValue + attribute[1].Trim('\'');
            }
        }
    }
}