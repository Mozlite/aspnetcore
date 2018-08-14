using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mozlite.Mvc.Templates.Codings
{
    /// <summary>
    /// IF语法呈现类。
    /// </summary>
    public class IfSyntaxWriter : SyntaxWriter<IfSyntax>
    {
        /// <summary>
        /// 将文档<paramref name="current"/>解析写入到写入器中。
        /// </summary>
        /// <param name="current">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="write">写入子项目。</param>
        protected override void WriteRazor(IfSyntax current, TextWriter writer, Action<Syntax, TextWriter, ViewDataDictionary> write)
        {
            writer.Write(current.Indent());
            writer.WriteLine("@if({0}){{", string.Join(", ", current.Parameters));
            foreach (var child in current)
            {
                write(child, writer, null);
            }

            writer.Write(current.Indent());
            writer.WriteLine("}");

            if (current.ElseIf != null)
            {
                foreach (var elseif in current.ElseIf)
                {
                    writer.Write(current.Indent());
                    writer.WriteLine("else if({0}){{", string.Join(", ", elseif.Parameters));
                    foreach (var child in elseif)
                    {
                        write(child, writer, null);
                    }

                    writer.Write(current.Indent());
                    writer.WriteLine("}");
                }
            }

            if (current.Else != null)
            {
                writer.Write(current.Indent());
                writer.WriteLine("else{");
                foreach (var child in current.Else)
                {
                    write(child, writer, null);
                }

                writer.Write(current.Indent());
                writer.WriteLine("}");
            }
        }

        /// <summary>
        /// 将文档<paramref name="current"/>解析写入到写入器中。
        /// </summary>
        /// <param name="current">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="model">当前模型实例。</param>
        /// <param name="write">写入子项目。</param>
        protected override void WriteHtml(IfSyntax current, TextWriter writer, ViewDataDictionary model, Action<Syntax, TextWriter, ViewDataDictionary> write)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 初始化类<see cref="IfSyntaxWriter"/>。
        /// </summary>
        public IfSyntaxWriter() : base("@if")
        {
        }
    }
}