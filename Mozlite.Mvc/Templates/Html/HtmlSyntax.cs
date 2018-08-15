using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mozlite.Mvc.Templates.Html
{
    /// <summary>
    /// HTML语法。
    /// </summary>
    public class HtmlSyntax : Syntax
    {
        /// <summary>
        /// 参数。
        /// </summary>
        public IDictionary<string, string> Attributes { get; internal set; }

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
                    builder.AppendFormat(" {0}=\"{1}\"", attribute.Key, attribute.Value);
                }
            }
            if (IsBlock)
            {
                if (this.Any())
                {
                    builder.AppendLine(">");
                    foreach (var syntax in this)
                    {
                        builder.Append(syntax);
                    }
                    builder.Append(indent).AppendFormat("</{0}>", Name).AppendLine();
                }
                else
                {
                    builder.AppendFormat("></{0}>", Name).AppendLine();
                }
            }
            else
            {
                builder.Append("/>").AppendLine();
            }
            return builder.ToString();
        }
    }
}