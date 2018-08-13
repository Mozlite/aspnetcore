using System.Text;

namespace Mozlite.Mvc.Templates.Html
{
    /// <summary>
    /// 代码块。
    /// </summary>
    public class CodeHtmlSyntax : HtmlSyntax
    {
        /// <summary>
        /// 初始化类<see cref="CodeHtmlSyntax"/>。
        /// </summary>
        public CodeHtmlSyntax()
        {
            IsBlock = true;
        }

        /// <summary>
        /// 代码块。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 当前语法的呈现字符串。
        /// </summary>
        /// <returns>返回当前语法的呈现字符串。</returns>
        public override string ToString()
        {
            var indent = Indent();
            var builder = new StringBuilder();
            if (Declarings.Count > 0)
                builder.Append(indent).AppendLine(Declarings.Join("\r\n" + indent));
            builder.Append(indent).AppendFormat("<{0}", Name);
            if (Attributes?.Count > 0)
            {
                foreach (var attribute in Attributes)
                {
                    builder.AppendFormat(" {0}={1}", attribute.Key, attribute.Value);
                }
            }

            if (string.IsNullOrWhiteSpace(Code))
            {
                builder.AppendFormat("></{0}>", Name).AppendLine();
            }
            else
            {
                builder.AppendLine(">");
                builder.AppendLine(Code);
                builder.Append(indent).AppendFormat("</{0}>", Name).AppendLine();
            }
            return builder.ToString();
        }
    }
}