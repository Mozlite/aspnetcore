using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 语法管理实现类。
    /// </summary>
    public class SyntaxManager : ISyntaxManager
    {
        /// <summary>
        /// 解析模板语法。
        /// </summary>
        /// <param name="source">模板源代码。</param>
        /// <returns>返回当前文档实例。</returns>
        public virtual DocumentSyntax Parse(string source)
        {
            var reader = new CodeReader(source);
            var document = new DocumentSyntax();
            Parse(reader, document);
            return document;
        }

        private IEnumerable<Declaring> ParseDeclarings(CodeReader reader)
        {
            var declarings = new List<Declaring>();
            //先读取声明
            while (reader.IsNextNonWhiteSpace('!'))
            {
                var declaring = new Declaring();
                declaring.Name = reader.ReadName();
                if (reader.IsNextNonWhiteSpace("({"))
                {
                    declaring.Attributes = reader.ReadAttributes();
                    declaring.DeclaringType = DeclaringType.Attributes;
                }
                else if (reader.IsNextNonWhiteSpace('('))
                {
                    declaring.Parameters = reader.ReadParameters();
                    declaring.DeclaringType = DeclaringType.Parameters;
                }
                else
                {
                    declaring.Declare = reader.ReadUntil("\r\n")?.Trim(' ', ';');
                    declaring.DeclaringType = DeclaringType.Declare;
                }
                declarings.Add(declaring);
            }
            return declarings;
        }

        /// <summary>
        /// 解析模板语法。
        /// </summary>
        /// <param name="reader">当前读取实例。</param>
        /// <param name="syntax">当前语法节点。</param>
        protected virtual void Parse(CodeReader reader, Syntax syntax)
        {
            var declares = ParseDeclarings(reader);
            //读取代码
            while (reader.MoveNext())
            {
                if (reader.Current == '@')
                {
                    reader.Skip();
                    var name = reader.ReadName();
                    if (reader.IsNextNonWhiteSpace('('))
                    {
                        var current = new FunctionCodeSyntax();
                        current.Name = name;
                        //读取参数
                        if (!reader.IsNextNonWhiteSpace(')'))
                            current.Parameters = reader.ReadParameters();
                        current.Declarings.AddRange(declares);
                        syntax.Append(current);
                    }
                    else
                    {
                        var current = new PropertyCodeSyntax();
                        current.Name = name;
                        current.Declarings.AddRange(declares);
                        syntax.Append(current);
                    }
                }
                else if (reader.Current == '}')
                {
                    reader.Skip();
                    break;
                }
                else
                {
                    var current = new HtmlSyntax();
                    current.Name = reader.ReadName();
                    current.Attributes = reader.ReadAttributes();
                    current.Declarings.AddRange(declares);
                    syntax.Append(current);
                    //函数结束
                    if (reader.IsNextNonWhiteSpace(';'))
                        continue;
                    if (reader.IsNextNonWhiteSpace('{'))
                    {
                        current.IsBlock = true;
                        Parse(reader, current);
                    }
                }
            }
        }

        /// <summary>
        /// 加载模板文件。
        /// </summary>
        /// <param name="path">文件物理路径。</param>
        /// <returns>返回当前文档实例。</returns>
        public virtual DocumentSyntax Load(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
                return Parse(reader.ReadToEnd());
        }
    }
}