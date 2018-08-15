using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Mozlite.Mvc.Templates.Codings;
using Mozlite.Mvc.Templates.Declarings;
using Mozlite.Mvc.Templates.Html;

namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 语法管理实现类。
    /// </summary>
    public class SyntaxManager : ISyntaxManager
    {
        private readonly IDictionary<string, ISyntaxWriter> _syntaxWriters;
        private readonly IDictionary<string, IDeclaringWriter> _declaringWriters;

        /// <summary>
        /// 初始化类<see cref="SyntaxManager"/>。
        /// </summary>
        /// <param name="declaringWriters">声明列表。</param>
        /// <param name="syntaxWriters">语法写入器接口。</param>
        public SyntaxManager(IEnumerable<IDeclaringWriter> declaringWriters, IEnumerable<ISyntaxWriter> syntaxWriters)
        {
            _syntaxWriters = syntaxWriters.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            _declaringWriters = declaringWriters.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 解析模板语法。
        /// </summary>
        /// <param name="source">模板源代码。</param>
        /// <returns>返回当前文档实例。</returns>
        public virtual Syntax Parse(string source)
        {
            var reader = new CodeReader(source);
            var document = new Document();
            Parse(reader, document);
            return document;
        }

        /// <summary>
        /// 解析模板语法。
        /// </summary>
        /// <param name="reader">当前读取实例。</param>
        /// <param name="syntax">当前语法节点。</param>
        protected virtual void Parse(CodeReader reader, Syntax syntax)
        {
            var declares = reader.ReadDeclarings();
            //读取代码
            while (reader.MoveNext())
            {
                if (reader.Current == '@')
                {
                    var name = reader.ReadName();
                    switch (name.ToLower())
                    {
                        case "@":
                            {
                                reader.MoveNext();
                                if (reader.Current == '{' || reader.Current == '(')
                                {//代码块@{}/@()
                                    var current = new SourceCodeSyntax();
                                    current.Name = name;
                                    if (reader.Current == '(')
                                        current.Source = reader.ReadBlock('(', ')');
                                    else
                                        current.Source = reader.ReadBlock('{', '}');
                                    current.Declarings.AddRange(declares);
                                    syntax.Append(current);
                                }
                            }
                            continue;
                        case "@if":
                            {
                                ReadIf(reader, syntax);
                            }
                            continue;
                        default:
                            {
                                if (reader.IsNextNonWhiteSpace(';') || !reader.IsNextNonWhiteSpace('(', false))
                                {//@属性
                                    var property = new CodeSyntax();
                                    property.Name = name;
                                    property.Declarings.AddRange(declares);
                                    syntax.Append(property);
                                }
                                else
                                {
                                    var current = new FunctionSyntax();
                                    current.Name = name;
                                    //读取参数
                                    if (!reader.IsNextNonWhiteSpace(')'))
                                        current.Parameters = reader.ReadParameters();
                                    current.Declarings.AddRange(declares);
                                    syntax.Append(current);
                                    //函数结束
                                    ParseChildren(reader, current);
                                }
                                continue;
                            }
                    }
                }

                if (reader.Current == '}')
                {
                    reader.Skip();
                    break;
                }

                if (reader.Current == ';')
                {
                    reader.Skip();
                }
                else
                {
                    var name = reader.ReadName();
                    switch (name.ToLower())
                    {
                        case "script":
                        case "style":
                            {
                                var current = new CodeHtmlSyntax();
                                current.Name = name;
                                current.Attributes = reader.ReadAttributes();
                                current.Declarings.AddRange(declares);
                                current.Code = reader.ReadBlock('{', '}').Trim('{', '}', ' ');
                                syntax.Append(current);
                            }
                            continue;
                        default:
                            {
                                var current = new HtmlSyntax();
                                current.Name = name;
                                current.Attributes = reader.ReadAttributes();
                                current.Declarings.AddRange(declares);
                                syntax.Append(current);
                                //函数结束
                                ParseChildren(reader, current);
                            }
                            continue;
                    }
                }
            }
        }

        /// <summary>
        /// 读取if(){}else if(){}else{}语句。
        /// </summary>
        private void ReadIf(CodeReader reader, Syntax syntax)
        {
            var current = new IfSyntax();
            current.Name = "@if";
            current.Parameters = reader.ReadParameters();
            ParseChildren(reader, current);
            //读取elseif语句
            while (reader.IsNextNonWhiteSpace("elseif", stringComparison: StringComparison.OrdinalIgnoreCase))
            {
                var elseif = new IfSyntax();
                elseif.Name = "elseif";
                elseif.Parameters = reader.ReadParameters();
                ParseChildren(reader, elseif);
                if (current.ElseIf == null)
                    current.ElseIf = new List<IfSyntax>();
                current.ElseIf.Add(elseif);
            }
            //读取elseif语句
            if (reader.IsNextNonWhiteSpace("else",
                stringComparison: StringComparison.OrdinalIgnoreCase))
            {
                var @else = new CodeSyntax();
                @else.Name = "else";
                ParseChildren(reader, @else);
                current.Else = @else;
            }
            syntax.Append(current);
        }

        [DebuggerStepThrough]
        private void ParseChildren(CodeReader reader, Syntax syntax)
        {
            if (reader.IsNextNonWhiteSpace(';'))
                return;
            if (reader.IsNextNonWhiteSpace('{'))
            {
                syntax.IsBlock = true;
                Parse(reader, syntax);
            }
        }

        /// <summary>
        /// 加载模板文件。
        /// </summary>
        /// <param name="path">文件物理路径。</param>
        /// <returns>返回当前文档实例。</returns>
        public virtual Syntax Load(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
                return Parse(reader.ReadToEnd());
        }

        /// <summary>
        /// 将文档<paramref name="syntax"/>解析写入到写入器中。
        /// </summary>
        /// <param name="syntax">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="viewData">当前模型实例。</param>
        public void Write(Syntax syntax, TextWriter writer, Action<ViewDataDictionary> viewData)
        {
            var model = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            viewData?.Invoke(model);
            if (syntax is Document)
            {
                foreach (var child in syntax)
                {
                    WriteSyntax(child, writer, model);
                }
            }
            else
                WriteSyntax(syntax, writer, model);
        }

        /// <summary>
        /// 将语法解析后写入到写入器中。
        /// </summary>
        /// <param name="syntax">当前文档实例。</param>
        /// <param name="writer">写入器实例对象。</param>
        /// <param name="model">当前模型实例。</param>
        protected virtual void WriteSyntax(Syntax syntax, TextWriter writer, ViewDataDictionary model)
        {
            foreach (var declaring in syntax.Declarings)
            {
                if (_declaringWriters.TryGetValue(declaring.Name, out var declaringWriter))
                    declaringWriter.Write(writer, declaring, model);
            }
            if (!_syntaxWriters.TryGetValue(syntax.Name, out var syntaxWriter))
                syntaxWriter = _syntaxWriters[DefaultSyntaxWriter.DefaultName];
            syntaxWriter.Write(syntax, writer, model, WriteSyntax);
        }

        /// <summary>
        /// 获取呈现的代码字符串。
        /// </summary>
        /// <param name="syntax">当前文档实例。</param>
        /// <param name="viewData">当前模型实例。</param>
        /// <returns>返回呈现的代码字符串。</returns>
        public string Render(Syntax syntax, Action<ViewDataDictionary> viewData)
        {
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
                Write(syntax, writer, viewData);
            return builder.ToString();
        }

        private class Document : Syntax
        {

        }
    }
}