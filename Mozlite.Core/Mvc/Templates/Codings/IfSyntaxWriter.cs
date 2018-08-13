using System;
using System.IO;

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
        public virtual void Write(Syntax syntax, TextWriter writer, object model, Action<Syntax, TextWriter, object> write)
        {
            if (syntax is IfSyntax ifSyntax)
            {
                writer.Write(ifSyntax.Indent());
                writer.WriteLine("@if({0}){{", string.Join(", ", ifSyntax.Parameters));
                foreach (var child in syntax)
                {
                    write(child, writer, model);
                }

                writer.Write(ifSyntax.Indent());
                writer.WriteLine("}");

                if (ifSyntax.ElseIf != null)
                {
                    foreach (var elseif in ifSyntax.ElseIf)
                    {
                        writer.Write(ifSyntax.Indent());
                        writer.WriteLine("else if({0}){{", string.Join(", ", elseif.Parameters));
                        foreach (var child in elseif)
                        {
                            write(child, writer, model);
                        }

                        writer.Write(ifSyntax.Indent());
                        writer.WriteLine("}");
                    }
                }

                if (ifSyntax.Else != null)
                {
                    writer.Write(ifSyntax.Indent());
                    writer.WriteLine("else{");
                    foreach (var child in ifSyntax.Else)
                    {
                        write(child, writer, model);
                    }

                    writer.Write(ifSyntax.Indent());
                    writer.WriteLine("}");
                }
            }
        }
    }
}