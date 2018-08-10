using System.IO;
using Mozlite.Mvc.Templates.Html;

namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 默认写入器。
    /// </summary>
    public class DefaultSyntaxWriter : ISyntaxWriter
    {
        /// <summary>
        /// 默认名称。
        /// </summary>
        public const string DefaultName = ":name:";

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name => DefaultName;

        /// <summary>
        /// 将文档<paramref name="syntax"/>解析写入到写入器中。
        /// </summary>
        /// <param name="syntax">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="model">当前模型实例。</param>
        public virtual void BeginWrite(Syntax syntax, TextWriter writer, object model)
        {
            if (syntax is HtmlSyntax html)
            {
                writer.Write(html.Indent());
                writer.Write("<");
                writer.Write(html.Name);
                if (html.Attributes != null)
                {
                    foreach (var htmlAttribute in html.Attributes)
                    {
                        writer.Write(" ");
                        writer.Write(htmlAttribute.Key);
                        writer.Write("=");
                        writer.Write(htmlAttribute.Value);
                    }
                }
                if (syntax.IsBlock)
                    writer.WriteLine(">");
                else
                    writer.WriteLine("/>");
            }
            else
            {
                writer.Write(syntax.ToString());
            }
        }

        /// <summary>
        /// 将文档<paramref name="syntax"/>解析写入到写入器中。
        /// </summary>
        /// <param name="syntax">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="model">当前模型实例。</param>
        public virtual void EndWrite(Syntax syntax, TextWriter writer, object model)
        {
            if (!syntax.IsBlock)
                return;

            if (syntax is HtmlSyntax html)
            {
                writer.Write(html.Indent());
                writer.Write("</");
                writer.Write(html.Name);
                writer.WriteLine(">");
            }
            else
            {
                writer.Write(syntax.Indent());
                writer.WriteLine("}");
            }
        }
    }
}