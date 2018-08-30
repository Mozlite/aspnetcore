using System;
using System.Linq;
using System.Text;

namespace Mozlite.Utils
{
    /// <summary>
    /// 源码读取器。
    /// </summary>
    public class SourceReader
    {
        private readonly string _source;
        private int _index;
        private readonly int _length;
        /// <summary>
        /// 空字符。
        /// </summary>
        public static readonly char NullChar = '\0';
        /// <summary>
        /// 转义符。
        /// </summary>
        public static readonly char EscapeChar = '\\';

        /// <summary>
        /// 初始化类<see cref="SourceReader"/>。
        /// </summary>
        /// <param name="source">源代码。</param>
        public SourceReader(string source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _length = source.Length;
        }

        /// <summary>
        /// 过滤空字符串，并且判断是否可读。
        /// </summary>
        /// <returns>返回判断结果。</returns>
        public bool MoveNext()
        {
            while (_length > _index)
            {
                if (!char.IsWhiteSpace(_source, _index))
                    return true;
                _index++;
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
            var index = _index + 1;
            return _length > index &&
                   _source[index] == current;
        }

        /// <summary>
        /// 判断是否下一个非空字符是否为当前字符。
        /// </summary>
        /// <param name="current">当前非空字符。</param>
        /// <param name="skip">跳过当前位置。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsNextNonWhiteSpace(char current, bool skip = true)
        {
            var index = _index + 1;
            while (_length > index)
            {
                if (!char.IsWhiteSpace(_source, index))
                    break;
                index++;
            }

            if (_source[index] == current)
            {
                if (skip) _index = index + 1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断是否下一个字符串是否为当前字符串。
        /// </summary>
        /// <param name="current">当前字符串。</param>
        /// <param name="comparison">字符串对比模式。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsNext(string current, StringComparison comparison = StringComparison.Ordinal)
        {
            return _length > _index + current.Length &&
                   _source.IndexOf(current, _index, comparison) == _index;
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
            var index = _index + 1;//判断开始的位置
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

            if (builder.ToString().Equals(current, stringComparison))
            {
                if (skip) _index = index + 1;
                return true;
            }

            return false;
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
        /// 跳过<paramref name="current"/>字符及以前的字符串。
        /// </summary>
        /// <param name="current">跳过字符。</param>
        public void Skip(char current)
        {
            while (_length > _index)
            {
                if (_source[_index++] == current)
                    break;
            }
        }

        /// <summary>
        /// 当前字符，如果已经到末尾返回空字符。
        /// </summary>
        public char Current => _length > _index ? _source[_index] : NullChar;

        /// <summary>
        /// 隐式转换为字符串实例。
        /// </summary>
        /// <param name="reader">当前源码读取实例。</param>
        public static implicit operator string(SourceReader reader) => reader._source;

        /// <summary>
        /// 返回当前可读取的字符串。
        /// </summary>
        /// <returns>返回可读取的字符串。</returns>
        public override string ToString()
        {
            if (_index < _length)
                return _source.Substring(_index);
            return null;
        }

        /// <summary>
        /// 读取引用块。
        /// </summary>
        /// <param name="quote">引用符号：'"','`','\''。</param>
        /// <param name="builder">字符串构建实例。</param>
        /// <returns>返回当前引用块字符串。</returns>
        public string ReadQuote(char quote, StringBuilder builder = null)
        {
            builder = builder ?? new StringBuilder();
            if (quote == Current)
                _index++;
            builder.Append(quote);
            var last = NullChar;
            while (_length > _index)
            {
                var current = Current;
                if (current == EscapeChar)
                {
                    if (last == EscapeChar)
                    {
                        builder.Append(EscapeChar).Append(current);
                        last = NullChar;
                    }
                    else
                        last = current;
                }
                else if (current == quote)
                {
                    if (last == EscapeChar)
                    {
                        builder.Append(EscapeChar).Append(current);
                        last = NullChar;
                    }
                    else
                    {
                        _index++;
                        break;
                    }
                }
                else
                {
                    builder.Append(current);
                }
                _index++;
            }

            builder.Append(quote);
            return builder.ToString();
        }

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
                if (current.IsQuote())
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
                if (current.IsQuote())
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
                if (current.IsQuote())
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
                if (current.IsQuote())
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
    }
}