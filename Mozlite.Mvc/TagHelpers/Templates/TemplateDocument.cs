using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 模板文档实例。
    /// </summary>
    public class TemplateDocument : TemplateElement
    {
        private readonly string _source;
        private int _index;
        private readonly int _position;
        private readonly Stack<TemplateElementBase> _stack = new Stack<TemplateElementBase>();
        private static readonly Regex _whiteSpace = new Regex("([{;}])(\\r)?\\n\\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex _regex = new Regex("<script.*?>(.*?)</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 脚本。
        /// </summary>
        public string Scripts { get; }

        /// <summary>
        /// 初始化类<see cref="TemplateDocument"/>。
        /// </summary>
        /// <param name="source">源代码。</param>
        public TemplateDocument(string source) : base(0, TemplateElementType.Document)
        {
            var scripts = new StringBuilder();
            //提取脚本
            source = _regex.Replace(source, match =>
            {
                var script = match.Groups[1].Value.Trim().Trim(';');
                script = _whiteSpace.Replace(script, "$1");
                scripts.Append(script).Append(";");
                return null;
            });
            Scripts = scripts.ToString();
            _source = source.Trim();
            PassHtmlOptionalWhitespace();
            while (_index < _source.Length)
            {
                _position = _index;
                ParseDocument(Current);
                PassHtmlOptionalWhitespace();
            }
            var children = _stack.ToList();
            children.Reverse();
            AddRange(children);
        }

        private void ParseDocument(char current)
        {
            //开始位置
            if (current == '{' && Next == '{')
            {//语法或代码节点开始
                var code = ReadCode();
                if (code[code.Length - 1] == '/')
                {//自闭和
                    var element = new TemplateSyntaxElement(code.Substring(0, code.Length - 1), _position);
                    element.IsSelfClosed = true;
                    _stack.Push(element);
                }
                else if (code[0] == '/')
                {//结束
                    code = code.Substring(1).Trim();
                    var children = new List<TemplateElementBase>();
                    var element = _stack.Pop();
                    while (true)
                    {
                        if (element is TemplateCodeElement ce && ce.Keyword == code)
                        {
                            var syntax = new TemplateSyntaxElement(ce);
                            children.Reverse();
                            syntax.AddRange(children);
                            _stack.Push(syntax);
                            return;
                        }
                        children.Add(element);
                        if (_stack.Count == 0)
                        {
                            children.Reverse();
                            AddRange(children);
                            return;
                        }
                        element = _stack.Pop();
                    }
                }
                else
                {//代码块
                    _stack.Push(new TemplateCodeElement(code, _position));
                }
            }
            else if (current == '<')
            {//html节点开始
                if (Next == '/')
                {//结束节点
                    _index += 2;
                    var name = ReadTagName();
                    _index++;//跳过>符号
                    var children = new List<TemplateElementBase>();
                    var element = _stack.Pop();
                    while (true)
                    {
                        if (element is TemplateHtmlElement ce && ce.TagName == name)
                        {
                            children.Reverse();
                            ce.AddRange(children);
                            _stack.Push(ce);
                            return;
                        }
                        children.Add(element);
                        if (_stack.Count == 0)
                        {
                            children.Reverse();
                            AddRange(children);
                            return;
                        }
                        element = _stack.Pop();
                    }
                }
                else
                {
                    _index++;
                    var element = new TemplateHtmlElement(ReadTagName(), _position);
                    PassOptionalWhitespace();
                    do
                    {
                        if (IsHtmlEnd())
                            break;
                        var attribute = ReadAttribute();
                        element[attribute.Name] = attribute;
                    }
                    while (true);
                    element.IsSelfClosed = Current == '/';
                    _stack.Push(element);
                    if (element.IsSelfClosed)
                        _index++;
                    _index++;
                }
            }
            else
                _stack.Push(ReadText());
        }

        private bool IsHtmlEnd() => Current == '>' || (Current == '/' && Next == '>');

        private TemplateHtmlAttribute ReadAttribute()
        {
            var sb = new StringBuilder();
            string name;
            while (_index < _source.Length)
            {
                if (Current == ' ' || Current == '=' || Current == '>' || (Current == '/' && Next == '>'))
                {
                    PassOptionalWhitespace();
                    if (Current != '=')
                    {//没有值的属性：disabled,readonly等，转换后disabled="disabled"。
                        name = sb.ToString().Trim();
                        return new TemplateHtmlAttribute(name, name);
                    }
                    _index++;
                    break;
                }
                sb.Append(Current);
                _index++;
            }
            name = sb.ToString().Trim();

            PassOptionalWhitespace();
            //属性引号
            var quote = Current;
            if (quote != '\'' && quote != '"')
                quote = ' ';
            else
                _index++;
            if (Current == '{' && Next == '{')
            {//代码段
                var attribute = new TemplateHtmlCodeAttribute(name, ReadCode());
                _index++;//跳过属性引号
                PassOptionalWhitespace();
                return attribute;
            }

            sb = new StringBuilder();
            while (_index < _source.Length)
            {
                if (Current == quote || Current == '>' || (Current == '/' && Next == '>'))
                {
                    if (Current == quote)
                        _index++;
                    break;
                }
                sb.Append(Current);
                _index++;
            }
            PassOptionalWhitespace();
            return new TemplateHtmlAttribute(name, sb.ToString());
        }

        /// <summary>
        /// 读取代码片段。
        /// </summary>
        private string ReadCode()
        {
            var sb = new StringBuilder();
            _index += 2;//去掉{{
            while (_index < _source.Length)
            {
                if (Current == '\'' || Current == '"')
                {
                    sb.Append(ReadQuote(Current));
                    continue;
                }
                if (Current == '}' && Next == '}')
                    break;
                sb.Append(Current);
                _index++;
            }
            _index += 2;//去掉}}
            return sb.ToString().Trim();
        }

        /// <summary>
        /// 读取HTML节点。
        /// </summary>
        private string ReadTagName()
        {
            PassOptionalWhitespace();
            var sb = new StringBuilder();
            while (_index < _source.Length)
            {
                if (Current == ' ' || Current == '>')
                    break;
                sb.Append(Current);
                _index++;
            }
            return sb.ToString().Trim().ToLower();
        }

        /// <summary>
        /// 过滤空格。
        /// </summary>
        private void PassOptionalWhitespace()
        {
            while (_index < _source.Length && (Current == ' ' || Current == '\t'))
            {
                _index++;
            }
        }

        /// <summary>
        /// 过滤空格。
        /// </summary>
        private void PassHtmlOptionalWhitespace()
        {
            while (_index < _source.Length && (Current == ' ' || Current == '\t' || Current == '\r' || Current == '\n'))
            {
                _index++;
            }
        }

        /// <summary>
        /// 读取字符串。
        /// </summary>
        private TemplateTextElement ReadText()
        {
            var sb = new StringBuilder();
            while (_index < _source.Length)
            {
                if ((Current == '{' && Next == '{') ||//代码结束
                    (Current == '<')//Html结束
                    )
                    break;
                sb.Append(Current);
                _index++;
            }
            return new TemplateTextElement(sb.ToString(), _position);
        }

        /// <summary>
        /// 读取引用字符串。
        /// </summary>
        private StringBuilder ReadQuote(char quote)
        {
            /*
                xx == 'sdf{{sdfsdf';
                xx == 'sdf\'sd{{}}fsdf';
                xx == 'sdfsd{{}}fsdf\\';
             */
            var sb = new StringBuilder();
            sb.Append(quote);
            _index++;
            while (_index < _source.Length)
            {
                if (quote == Current)
                    break;
                if (Current == '\\')//转义符号
                {
                    sb.Append(Current);
                    _index++;
                    continue;
                }
                sb.Append(Current);
                _index++;
            }
            sb.Append(quote);
            _index++;
            return sb;
        }

        private const char None = '\0';

        private char Current => _source[_index];

        private char Next => _index + 1 < _source.Length ? _source[_index + 1] : None;

        /// <summary>
        /// 生成脚本。
        /// </summary>
        /// <param name="executor">脚本语法解析器。</param>
        /// <returns>返回生成后的脚本。</returns>
        public override string ToJsString(ITemplateExecutor executor)
        {
            var sb = new StringBuilder();
            foreach (var element in this)
            {
                sb.Append(element.ToJsString(executor));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成HTML代码。
        /// </summary>
        /// <param name="executor">语法解析器。</param>
        /// <param name="instance">当前实例。</param>
        /// <param name="func">获取实例属性值。</param>
        /// <returns>返回HTML代码。</returns>
        public override string ToHtmlString(ITemplateExecutor executor, object instance, Func<object, string, object> func)
        {
            var sb = new StringBuilder();
            foreach (var element in this)
            {
                sb.Append(element.ToHtmlString(executor, instance, func));
            }
            return sb.ToString();
        }
    }
}