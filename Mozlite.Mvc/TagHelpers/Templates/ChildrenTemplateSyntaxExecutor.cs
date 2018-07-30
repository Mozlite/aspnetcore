using System;
using System.Linq;
using System.Text;
using Mozlite.Extensions;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// Children语句（递归）。
    /// </summary>
    public class ChildrenTemplateSyntaxExecutor : ITemplateSyntaxExecutor
    {
        /// <summary>
        /// 关键词。
        /// </summary>
        public string Keyword => "children";

        /// <summary>
        /// 通过当前对象执行语法所得到的字符串。
        /// </summary>
        /// <param name="element">属性实例对象。</param>
        /// <param name="executor">解析器接口。</param>
        /// <param name="instance">当前实例对象。</param>
        /// <param name="func">获取当前对象属性值的方法。</param>
        /// <returns>返回脚本代码</returns>
        public string End(TemplateSyntaxElement element, ITemplateExecutor executor, object instance, Func<object, string, object> func)
        {
            return null;
        }

        /// <summary>
        /// 通过当前对象执行语法所得到的字符串。
        /// </summary>
        /// <param name="element">属性实例对象。</param>
        /// <param name="executor">解析器接口。</param>
        /// <returns>返回脚本代码</returns>
        public string Begin(TemplateSyntaxElement element, ITemplateExecutor executor)
        {
            if (element.IsSelfClosed || !element.Any())
                return "if($model.count>0){$model.children.forEach(function(item){appender(item);});}";
            var sb = new StringBuilder();
            sb.Append("function children($model){");
            foreach (var node in element)
            {
                sb.Append(node.ToJsString(executor));
            }
            sb.Append("};");
            sb.Append("if($model.count>0){$model.children.forEach(function(item){children(item);});}");
            return sb.ToString();
        }

        /// <summary>
        /// 通过当前对象执行语法所得到的字符串。
        /// </summary>
        /// <param name="element">属性实例对象。</param>
        /// <param name="executor">解析器接口。</param>
        /// <param name="instance">当前实例对象。</param>
        /// <param name="func">获取当前对象属性值的方法。</param>
        /// <returns>返回脚本代码</returns>
        public string Begin(TemplateSyntaxElement element, ITemplateExecutor executor, object instance, Func<object, string, object> func)
        {
            var sb = new StringBuilder();
            if (instance is IParentable models)
            {
                if (element.IsSelfClosed || !element.Any())
                {
                    foreach (var model in models.Children)
                    {
                        sb.Append(element.Document.ToHtmlString(executor, model, func));
                    }
                }
                else
                {
                    foreach (var model in models.Children)
                    {
                        sb.Append(element.ToHtmlString(executor, model, func));
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 通过当前对象执行语法所得到的字符串。
        /// </summary>
        /// <param name="element">属性实例对象。</param>
        /// <param name="executor">解析器接口。</param>
        /// <returns>返回脚本代码</returns>
        public string End(TemplateSyntaxElement element, ITemplateExecutor executor)
        {
            return null;
        }
    }
}