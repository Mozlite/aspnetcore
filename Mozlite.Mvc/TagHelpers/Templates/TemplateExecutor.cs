using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 语法解析执行工厂实现类。
    /// </summary>
    public class TemplateExecutor : ITemplateExecutor
    {
        private readonly ConcurrentDictionary<string, ITemplateSyntaxExecutor> _executors;
        private readonly ConcurrentDictionary<string, ITemplateHtmlAttributeExecutor> _attributes;
        public TemplateExecutor(IEnumerable<ITemplateSyntaxExecutor> executors, IEnumerable<ITemplateHtmlAttributeExecutor> attributes)
        {
            _executors = new ConcurrentDictionary<string, ITemplateSyntaxExecutor>(executors.ToDictionary(x => x.Keyword));
            _attributes = new ConcurrentDictionary<string, ITemplateHtmlAttributeExecutor>(attributes.ToDictionary(x => "moz:" + x.Keyword));
        }

        /// <summary>
        /// 尝试获取语法解析器。
        /// </summary>
        /// <param name="element">元素实例对象。</param>
        /// <param name="executor">返回的解析器。</param>
        /// <returns>返回获取结果。</returns>
        public bool TryGetExecutor(TemplateSyntaxElement element, out ITemplateSyntaxExecutor executor)
        {
            return _executors.TryGetValue(element.Keyword, out executor);
        }

        /// <summary>
        /// 通过当前对象执行属性所得到的字符串。
        /// </summary>
        /// <param name="attributes">属性实例对象列表。</param>
        /// <param name="result">当前原有的属性列表。</param>
        /// <returns>返回脚本代码</returns>
        public string Execute(IEnumerable<TemplateHtmlCodeAttribute> attributes, Dictionary<string, string> result)
        {
            var sb = new StringBuilder();
            foreach (var attribute in attributes)
            {
                if (_attributes.TryGetValue(attribute.Name, out var executor))
                {
                    sb.Append(executor.Execute(attribute, result));
                }
                else
                {
                    sb.Append("html+=' ")
                        .Append(attribute.Name)
                        .Append("=\"'+")
                        .Append(attribute.Value)
                        .Append("+'\"';");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 通过当前对象执行属性所得到的字符串。
        /// </summary>
        /// <param name="attributes">属性实例对象列表。</param>
        /// <param name="result">当前原有的属性列表。</param>
        /// <param name="instance">当前实例对象。</param>
        /// <param name="func">获取当前对象属性值的方法。</param>
        public void Execute(IEnumerable<TemplateHtmlCodeAttribute> attributes, Dictionary<string, string> result, object instance, Func<object, string, object> func)
        {
            foreach (var attribute in attributes)
            {
                if (_attributes.TryGetValue(attribute.Name, out var executor))
                {
                    executor.Execute(attribute, result, instance);
                }
                else
                {
                    result[attribute.Name] = func(instance, attribute.Value)?.ToString();
                }
            }
        }
    }
}