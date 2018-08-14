using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mozlite.Mvc.Templates.Html
{
    /// <summary>
    /// 脚本。
    /// </summary>
    public class ScriptSyntaxWriter : ISyntaxWriter
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Name => "script";

        /// <summary>
        /// 将文档<paramref name="syntax"/>解析写入到写入器中。
        /// </summary>
        /// <param name="syntax">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="model">当前模型实例。</param>
        /// <param name="write">写入子项目。</param>
        public virtual void Write(Syntax syntax, TextWriter writer, ViewDataDictionary model, Action<Syntax, TextWriter, ViewDataDictionary> write)
        {
            if (syntax is CodeHtmlSyntax code)
            {
                writer.Write(code.Indent());
                writer.Write("<");
                writer.Write(code.Name);
                if (code.Attributes != null)
                {
                    foreach (var htmlAttribute in code.Attributes)
                    {
                        writer.Write(" ");
                        writer.Write(htmlAttribute.Key);
                        writer.Write("=");
                        writer.Write(htmlAttribute.Value);
                    }
                }
                writer.Write(">");
                if (string.IsNullOrWhiteSpace(code.Code))
                    writer.WriteLine("</{0}>", Name);
                else
                {
                    writer.Write(code.Indent());
                    writer.WriteLine(code.Code);
                    writer.Write(code.Indent());
                    writer.WriteLine("</{0}>", Name);
                }
            }
        }
    }
}