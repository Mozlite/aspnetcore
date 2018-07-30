using System;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 代码节点。
    /// </summary>
    public class TemplateCodeElement : TemplateElementBase
    {
        /// <summary>
        /// 初始化类<see cref="TemplateCodeElement"/>。
        /// </summary>
        /// <param name="source">源代码。</param>
        /// <param name="position">位置。</param>
        protected internal TemplateCodeElement(string source, int position) : base(position, TemplateElementType.Code)
        {
            var index = source.IndexOf(' ');
            if (index == -1)
            { Keyword = source; }
            else
            {
                Keyword = source.Substring(0, index);
                Args = source.Substring(index + 1).Trim();
            }
        }

        /// <summary>
        /// 参数。
        /// </summary>
        public string Args { get; }

        /// <summary>
        /// 语句关键字。
        /// </summary>
        public string Keyword { get; }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            if (Args == null) return "{{" + Keyword + "}}";
            return $"{{{Keyword} {Args}}}";
        }

        /// <summary>
        /// 生成脚本。
        /// </summary>
        /// <param name="executor">脚本语法解析器。</param>
        /// <returns>返回生成后的脚本。</returns>
        public override string ToJsString(ITemplateExecutor executor)
        {
            if (Args == null)
                return $"html+= {Keyword};";
            return $"html+= {Keyword} {Args};";
        }

        /// <summary>
        /// 生成HTML代码。
        /// </summary>
        /// <param name="executor">语法解析器。</param>
        /// <param name="instance">当前实例。</param>
        /// <param name="func">获取实例属性值。</param>
        /// <returns>返回HTML代码。</returns>
        public override string ToHtmlString(ITemplateExecutor executor, object instance, Func<object, string, object> func)
        {
            return func(instance, Keyword)?.ToString();
        }
    }
}