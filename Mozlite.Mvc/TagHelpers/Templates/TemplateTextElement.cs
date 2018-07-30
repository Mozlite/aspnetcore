using System;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 文本节点。
    /// </summary>
    public class TemplateTextElement : TemplateElementBase
    {
        private readonly string _source;
        /// <summary>
        /// 初始化类<see cref="TemplateTextElement"/>。
        /// </summary>
        /// <param name="source">源代码。</param>
        /// <param name="position">位置。</param>
        internal TemplateTextElement(string source, int position) : base(position, TemplateElementType.Text)
        {
            _source = source;
        }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString() => _source;

        /// <summary>
        /// 生成脚本。
        /// </summary>
        /// <param name="executor">脚本语法解析器。</param>
        /// <returns>返回生成后的脚本。</returns>
        public override string ToJsString(ITemplateExecutor executor)
        {
            return $"html+='{_source?.Replace("'", "\'")}';";
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
            return _source;
        }
    }
}