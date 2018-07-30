using System;
using System.Text;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 语句节点。
    /// </summary>
    public class TemplateSyntaxElement : TemplateElement
    {
        /// <summary>
        /// 初始化类<see cref="TemplateSyntaxElement"/>。
        /// </summary>
        /// <param name="source">源代码。</param>
        /// <param name="position">位置。</param>
        protected internal TemplateSyntaxElement(string source, int position) : base(position, TemplateElementType.Syntax)
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

        internal TemplateSyntaxElement(TemplateCodeElement code) : base(code.Position, TemplateElementType.Syntax)
        {
            Keyword = code.Keyword;
            Args = code.Args;
        }

        /// <summary>
        /// 参数。
        /// </summary>
        public string Args { get; }

        /// <summary>
        /// 语句关键字。
        /// </summary>
        public string Keyword { get; }

        /// <summary>
        /// 是否为自闭合节点。
        /// </summary>
        public bool IsSelfClosed { get; set; }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("{{").Append(Keyword);
            if (Args != null)
                builder.Append(" ").Append(Args);
            if (IsSelfClosed)
            {
                builder.AppendLine("/}}");
                return builder.ToString();
            }
            builder.AppendLine("}}");
            builder.Append(base.ToString());
            builder.Append("{{/").Append(Keyword)
                .AppendLine("}}");
            return builder.ToString();
        }

        /// <summary>
        /// 生成脚本。
        /// </summary>
        /// <param name="executor">脚本语法解析器。</param>
        /// <returns>返回生成后的脚本。</returns>
        public override string ToJsString(ITemplateExecutor executor)
        {
            if (!executor.TryGetExecutor(this, out var syntax))
                throw new Exception($"不支持此语法：{ToString()}!");
            var sb = new StringBuilder();
            sb.Append(syntax.Begin(this, executor));
            foreach (var child in this)
            {
                sb.Append(child.ToJsString(executor));
            }
            sb.Append(syntax.End(this, executor));
            return sb.ToString();
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
            if (!executor.TryGetExecutor(this, out var syntax))
                throw new Exception($"不支持此语法：{ToString()}!");
            var sb = new StringBuilder();
            sb.Append(syntax.Begin(this, executor, instance, func));
            foreach (var child in this)
            {
                sb.Append(child.ToHtmlString(executor, instance, func));
            }
            sb.Append(syntax.End(this, executor, instance, func));
            return sb.ToString();
        }
    }
}