using System.Collections.Generic;
using System.Text;

namespace Mozlite.Mvc.Templates.Codings
{
    /// <summary>
    /// IF语句。
    /// </summary>
    public class IfSyntax : FunctionSyntax
    {
        /// <summary>
        /// ElseIf语句。
        /// </summary>
        public List<IfSyntax> ElseIf { get; set; }

        /// <summary>
        /// Else语句。
        /// </summary>
        public CodeSyntax Else { get; set; }

        /// <summary>
        /// 当前语法的呈现字符串。
        /// </summary>
        /// <returns>返回当前语法的呈现字符串。</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Indent()).AppendFormat("@if({0}){{", string.Join(", ", Parameters)).AppendLine();
            foreach (var code in this)
            {
                builder.Append(code);
            }
            builder.Append(Indent()).AppendLine("}");

            if (ElseIf != null)
            {
                foreach (var elseif in ElseIf)
                {
                    builder.Append(Indent()).AppendFormat("else if({0}){{", string.Join(", ", elseif.Parameters)).AppendLine();
                    foreach (var code in elseif)
                    {
                        builder.Append(code);
                    }
                    builder.Append(Indent()).AppendLine("}");
                }
            }

            if (Else != null)
            {
                builder.Append(Indent()).AppendLine("else{");
                foreach (var code in Else)
                {
                    builder.Append(code);
                }
                builder.Append(Indent()).AppendLine("}");
            }

            return builder.ToString();
        }
    }
}