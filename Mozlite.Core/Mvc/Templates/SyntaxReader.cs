using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 语法读取器。
    /// </summary>
    public class SyntaxReader
    {
        private readonly string _source;
        private int _index;
        /// <summary>
        /// 初始化类<see cref="SyntaxReader"/>。
        /// </summary>
        /// <param name="source">源代码。</param>
        public SyntaxReader(string source)
        {
            _source = source?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// 过滤空字符串，并且判断是否可读。
        /// </summary>
        /// <returns>返回判断结果。</returns>
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
        public string ReadName()
        {
            var name = new StringBuilder();
            //关键词名称，必须组合的字母或数字
            while (_source.Length > _index)
            {
                if (char.IsLetterOrDigit(_source, _index) || Current == '_')//字母，数字，下划线
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
        /// 读取参数。
        /// </summary>
        /// <returns>返回参数列表。</returns>
        public List<string> ReadParameters()
        {
            var parameters = new List<string>();
            while (MoveNext())
            {
                parameters.Add(ReadQuoteValue());
                if (Current == ')')
                    return parameters;
                Skip();
            }
            return parameters;
        }

        /// <summary>
        /// 跳过<paramref name="size"/>位置。
        /// </summary>
        /// <param name="size">跳过位置量。</param>
        public void Skip(int size = 1)
        {
            _index += size;
        }

        /// <summary>
        /// 跳过<paramref name="current"/>字符。
        /// </summary>
        /// <param name="current">跳过字符。</param>
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
                return ReadQuote(current);
            }
            return ReadUntil(new[] { ',', end });
        }

        private string ReadQuoteName()
        {
            var current = Current;
            if (IsQuote(current))
            {
                Skip();
                return ReadQuote(current);
            }
            return ReadUntil(':');
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
            return builder.ToString().Trim(' ', quote);
        }

        private bool IsQuote(char current) => current == '"' || current == '`' || current == '\'';

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
        /// <returns>返回判断结果。</returns>
        public bool IsNextNonWhiteSpace(string current, bool skip = true)
        {
            if (MoveNext() && _source.IndexOf(current, _index, StringComparison.Ordinal) == _index)
            {
                if (skip) Skip(current.Length);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否下一个字符串是否为当前字符串。
        /// </summary>
        /// <param name="current">当前字符串。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsNext(string current)
        {
            return _source.Length > _index + current.Length &&
                   _source.IndexOf(current, _index, StringComparison.Ordinal) == _index;
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
    }
}