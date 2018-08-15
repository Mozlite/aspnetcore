using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Mozlite.Mvc.Templates.Codings;
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
        /// <param name="write">写入子项目。</param>
        public virtual void Write(Syntax syntax, TextWriter writer, ViewDataDictionary model, Action<Syntax, TextWriter, ViewDataDictionary> write)
        {
            if (syntax is HtmlSyntax html)
            {
                var builder = new StringBuilder();
                if (html.Attributes != null)
                {
                    foreach (var htmlAttribute in html.Attributes)
                    {
                        var key = htmlAttribute.Key;
                        var value = htmlAttribute.Value;
                        if (value[0] == '@')
                        {
                            if (model.TryGetValue(value.TrimStart('@'), out var data))
                                value = data?.ToString();
                            else
                                value = null;
                        }
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            if (key[0] == '.')
                                return;//不显示
                        }
                        else
                            builder.AppendFormat(" {0}=\"{1}\"", key.Trim('.'), value);
                    }
                }
                writer.Write(html.Indent());
                writer.Write("<");
                writer.Write(html.Name);
                writer.Write(builder.ToString());

                if (syntax.IsBlock)
                {
                    writer.WriteLine(">");
                    foreach (var child in syntax)
                    {
                        write(child, writer, model);
                    }
                    writer.Write(html.Indent());
                    writer.Write("</");
                    writer.Write(html.Name);
                    writer.WriteLine(">");
                }
                else
                    writer.WriteLine("/>");
            }
            else if (syntax is CodeSyntax code && model.TryGetValue(code.Name.TrimStart('@'), out var data))
            {
                writer.Write(data);
            }
        }
    }
}