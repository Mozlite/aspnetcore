using Mozlite.Mvc.Templates.Declarings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 语法读取器。
    /// </summary>
    public class CodeReader
    {
        private readonly string _source;
        private int _index;
        /// <summary>
        /// 初始化类<see cref="CodeReader"/>。
        /// </summary>
        /// <param name="source">源代码。</param>
        public CodeReader(string source)
        {
            _source = source?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// 过滤空字符串，并且判断是否可读。
        /// </summary>
        /// <returns>返回判断结果。</returns>
        [DebuggerStepThrough]
        public bool MoveNext()
        {
            while (_source.Length > _index)
            {
                if (!char.IsWhiteSpace(_source, _index))
                    return true;
                Skip();
            }
            return false;
        }

        /// <summary>
        /// 读取函数名称。
        /// </summary>
        /// <returns>返回函数名称。</returns>
        [DebuggerStepThrough]
        public string ReadName()
        {
            var name = new StringBuilder();
            //关键词名称，必须组合的字母或数字
            while (_source.Length > _index)
            {
                if (char.IsLetterOrDigit(_source, _index) || Current == '_' || Current == '@')//字母，数字，下划线
                {
                    name.Append(Current);
                    Skip();
                }
                else
                    break;
            }
            return name.ToString();
        }

        /// <summary>
        /// 读取声明列表。
        /// </summary>
        /// <returns>返回声明列表。</returns>
        [DebuggerStepThrough]
        public IEnumerable<Declaring> ReadDeclarings()
        {
            var declarings = new List<Declaring>();
            //先读取声明
            while (IsNextNonWhiteSpace('!'))
            {
                declarings.Add(new Declaring(this));
            }
            return declarings;
        }

        /// <summary>
        /// 读取参数。
        /// </summary>
        /// <returns>返回参数列表。</returns>
        public string ReadParameters() => ReadBlock('(', ')').Trim('(', ')', ' ');

        /// <summary>
        /// 跳过<paramref name="size"/>位置。
        /// </summary>
        /// <param name="size">跳过位置量。</param>
        [DebuggerStepThrough]
        public void Skip(int size = 1)
        {
            _index += size;
        }

        /// <summary>
        /// 跳过<paramref name="current"/>字符。
        /// </summary>
        /// <param name="current">跳过字符。</param>
        [DebuggerStepThrough]
        public void SkipUntil(char current)
        {
            var isCurrent = false;
            while (MoveNext())
            {
                if (isCurrent) break;
                if (Current == current)
                    isCurrent = true;
                Skip();
            }
        }

        /// <summary>
        /// 读取JSON属性。
        /// </summary>
        /// <returns>返回属性列表。</returns>
        public IDictionary<string, string> ReadAttributes()
        {
            if (IsNextNonWhiteSpace("()") || !IsNextNonWhiteSpace("({"))
                return null;
            var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            while (MoveNext())
            {
                var key = ReadQuoteName();
                Skip();
                var value = ReadQuoteValue('}');
                attributes[key] = value;
                if (Current == '}' && IsNextNonWhiteSpace("})"))
                    return attributes;
                SkipUntil(',');
            }
            return attributes;
        }

        private string ReadQuoteValue(char end = ')')
        {
            var current = Current;
            if (IsQuote(current))
            {
                Skip();
                return ReadQuote(current).Trim(current, ' ');
            }
            return ReadUntil(new[] { ',', end });
        }

        private string ReadQuoteName()
        {
            var current = Current;
            if (IsQuote(current))
            {
                Skip();
                return ReadQuote(current).Trim(current, ' ');
            }
            return ReadUntil(':').Trim();
        }

        private const char EscapeCharacter = '\\';
        private const char ZeroCharacter = '\0';
        /// <summary>
        /// 读取引用块。
        /// </summary>
        /// <param name="quote">引用符号。</param>
        /// <param name="builder">字符串构建实例。</param>
        /// <returns>返回当前引用块字符串。</returns>
        public string ReadQuote(char quote, StringBuilder builder = null)
        {
            builder = builder ?? new StringBuilder();
            if (quote == Current)
                Skip();
            builder.Append(quote);
            var last = ZeroCharacter;
            while (_source.Length > _index)
            {
                var current = Current;
                if (current == EscapeCharacter)
                {
                    if (last == EscapeCharacter)
                    {
                        builder.Append(EscapeCharacter).Append(current);
                        last = ZeroCharacter;
                    }
                    else
                        last = current;
                }
                else if (current == quote)
                {
                    if (last == EscapeCharacter)
                    {
                        builder.Append(EscapeCharacter).Append(current);
                        last = ZeroCharacter;
                    }
                    else
                    {
                        Skip();
                        break;
                    }
                }
                else
                {
                    builder.Append(current);
                }
                Skip();
            }

            builder.Append(quote);
            return builder.ToString();
        }

        [DebuggerStepThrough]
        private bool IsQuote(char current) => current == '"' || current == '`' || current == '\'';

        /// <summary>
        /// 读取代码块。
        /// </summary>
        /// <param name="start">块开始符号。</param>
        /// <param name="end">块结束符号。</param>
        /// <param name="builder">字符串构建实例。</param>
        /// <returns>返回当前字符串。</returns>
        public string ReadBlock(char start, char end, StringBuilder builder = null)
        {
            builder = builder ?? new StringBuilder();
            var blocks = 0;
            while (_source.Length > _index)
            {
                var current = Current;
                if (IsQuote(current))
                {
                    ReadQuote(current, builder);
                    continue;
                }

                builder.Append(current);
                _index++;

                if (current == start)
                    blocks++;
                else if (current == end)
                    blocks--;
                if (blocks <= 0)
                    break;
            }
            return builder.ToString();
        }

        /// <summary>
        /// 读取字符串。
        /// </summary>
        /// <param name="end">结束字符。</param>
        /// <param name="builder">字符串构建实例。</param>
        /// <returns>返回当前字符串。</returns>
        public string ReadUntil(char end, StringBuilder builder = null)
        {
            builder = builder ?? new StringBuilder();
            while (_source.Length > _index)
            {
                var current = Current;
                if (IsQuote(current))
                {
                    ReadQuote(current, builder);
                    continue;
                }
                if (current == end)
                    return builder.ToString();
                builder.Append(current);
                _index++;
            }
            return builder.ToString();
        }

        /// <summary>
        /// 读取字符串。
        /// </summary>
        /// <param name="end">结束字符串。</param>
        /// <param name="builder">字符串构建实例。</param>
        /// <returns>返回当前字符串。</returns>
        public string ReadUntil(string end, StringBuilder builder = null)
        {
            builder = builder ?? new StringBuilder();
            while (_source.Length > _index)
            {
                var current = Current;
                if (IsQuote(current))
                {
                    ReadQuote(current, builder);
                    continue;
                }
                if (current == end[0] && IsNext(end))
                    return builder.ToString();
                builder.Append(current);
                Skip();
            }
            return builder.ToString();
        }

        /// <summary>
        /// 读取字符串。
        /// </summary>
        /// <param name="ends">结束字符集。</param>
        /// <param name="builder">字符串构建实例。</param>
        /// <returns>返回当前字符串。</returns>
        public string ReadUntil(char[] ends, StringBuilder builder = null)
        {
            builder = builder ?? new StringBuilder();
            while (_source.Length > _index)
            {
                var current = Current;
                if (IsQuote(current))
                {
                    ReadQuote(current, builder);
                    continue;
                }
                if (ends.Any(x => x == current))
                    return builder.ToString();
                builder.Append(current);
                Skip();
            }
            return builder.ToString();
        }

        /// <summary>
        /// 判断下一个非空字符是否为当前字符。
        /// </summary>
        /// <param name="current">当前字符。</param>
        /// <param name="skip">是否将读取位置与当前位置。</param>
        /// <returns>返回判断结果。</returns>
        [DebuggerStepThrough]
        public bool IsNextNonWhiteSpace(char current, bool skip = true)
        {
            if (MoveNext() && Current == current)
            {
                if (skip) Skip();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否下一个非空字符串是否为当前字符串。
        /// </summary>
        /// <param name="current">当前字符串。</param>
        /// <param name="skip">是否将读取位置与当前位置。</param>
        /// <param name="stringComparison">字符串对比模式。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsNextNonWhiteSpace(string current, bool skip = true, StringComparison stringComparison = StringComparison.Ordinal)
        {
            var index = _index;//判断开始的位置
            var builder = new StringBuilder();
            while (_source.Length > index)
            {
                if (!char.IsWhiteSpace(_source, index))
                {
                    builder.Append(_source[index]);
                    if (builder.Length == current.Length)
                        break;
                }
                index++;
            }

            index++;
            if (builder.ToString().Equals(current, stringComparison))
            {
                if (skip) _index = index;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断是否下一个字符串是否为当前字符。
        /// </summary>
        /// <param name="current">当前字符。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsNext(char current)
        {
            return _source.Length > _index + 1 &&
                   _source[_index + 1] == current;
        }

        /// <summary>
        /// 判断是否下一个字符串是否为当前字符串。
        /// </summary>
        /// <param name="current">当前字符串。</param>
        /// <param name="stringComparison">字符串对比模式。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsNext(string current, StringComparison stringComparison = StringComparison.Ordinal)
        {
            return _source.Length > _index + current.Length &&
                   _source.IndexOf(current, _index, stringComparison) == _index;
        }

        /// <summary>
        /// 当前字符。
        /// </summary>
        public char Current
        {
            get
            {
                try
                {
                    return _source[_index];
                }
                catch
                {
                    return ZeroCharacter;
                }
            }
        }

        /// <summary>
        /// 返回当前可读取的字符串。
        /// </summary>
        /// <returns>返回可读取的字符串。</returns>
        public override string ToString()
        {
            if (_index < _source.Length)
                return _source.Substring(_index);
            return null;
        }
    }
}