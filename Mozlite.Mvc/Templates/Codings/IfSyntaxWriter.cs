using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mozlite.Mvc.Templates.Codings
{
    /// <summary>
    /// IF语法呈现类。
    /// </summary>
    public class IfSyntaxWriter : ISyntaxWriter
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public string Name => "@if";

        /// <summary>
        /// 将文档<paramref name="syntax"/>解析写入到写入器中。
        /// </summary>
        /// <param name="syntax">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="model">当前模型实例。</param>
        /// <param name="write">写入子项目。</param>
        public void Write(Syntax syntax, TextWriter writer, ViewDataDictionary model, Action<Syntax, TextWriter, ViewDataDictionary> write)
        {
            if (syntax is IfSyntax current)
            {
                writer.Write(current.Indent());
                writer.WriteLine("@if({0}){{", current.Parameters);
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
                        writer.WriteLine("else if({0}){{", elseif.Parameters);
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
        }
    }
}