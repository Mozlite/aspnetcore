using System;
using System.Linq;
using System.Text;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 字符串。
    /// </summary>
    public class TemplateString
    {
        private readonly string _source;
        private int _index;
        /// <summary>
        /// 初始化类<see cref="TemplateString"/>。
        /// </summary>
        /// <param name="source">当前字符串。</param>
        public TemplateString(string source)
        {
            _source = source;
        }

        /// <summary>
        /// 读取直到碰到<paramref name="curs"/>字符之一，“()”和字符串优先读取。
        /// </summary>
        /// <param name="curs">当前字符。</param>
        /// <returns>返回读取的字符串。</returns>
        public string ReadUntil(params char[] curs)
        {
            var sb = new StringBuilder();
            while (CanRead)
            {
                if (curs.Contains(Current))
                {
                    _index++;
                    return sb.ToString();
                }
                if (Current == '\'' || Current == '"')
                {
                    sb.Append(ReadQuote(Current));
                }
                else if (Current == '(')
                {
                    sb.Append("(").Append(ReadBlock()).Append(')');
                }
                else
                {
                    sb.Append(Current);
                }
                _index++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 读取直到碰到两个链接字符。
        /// </summary>
        /// <param name="cur">当前字符。</param>
        /// <param name="next">下一个字符。</param>
        /// <returns>返回读取的字符串。</returns>
        public string ReadNext(char cur, char next)
        {
            var sb = new StringBuilder();
            while (CanRead)
            {
                if (Current == cur && Next == next)
                {
                    _index += 2;
                    return sb.ToString();
                }
                if (Current == '\'' || Current == '"')
                {
                    sb.Append(ReadQuote(Current));
                }
                else if (Current == '(')
                {
                    sb.Append("(").Append(ReadBlock()).Append(')');
                }
                else
                {
                    sb.Append(Current);
                }
                _index++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 开始读取，将位置设为0。
        /// </summary>
        public void Begin()
        {
            _index = 0;
        }

        /// <summary>
        /// 读取直到碰到<paramref name="cur"/>字符，“()”和字符串优先读取。
        /// </summary>
        /// <param name="cur">当前字符。</param>
        /// <returns>返回读取的字符串。</returns>
        public string ReadUntil(char cur)
        {
            var sb = new StringBuilder();
            while (CanRead)
            {
                if (Current == cur)
                {
                    _index++;
                    return sb.ToString();
                }
                if (Current == '\'' || Current == '"')
                {
                    sb.Append(ReadQuote(Current));
                }
                else if (Current == '(')
                {
                    sb.Append("(").Append(ReadBlock()).Append(')');
                }
                else
                {
                    sb.Append(Current);
                }
                _index++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 读取包含在<paramref name="text"/>中的字符。
        /// </summary>
        /// <param name="text">合法字符集合。</param>
        /// <returns>返回读取的字符串。</returns>
        public string Read(string text)
        {
            var sb = new StringBuilder();
            while (CanRead)
            {
                if (!text.Contains(Current))
                    break;
                sb.Append(Current);
                _index++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 判断当前位置是否合法。
        /// </summary>
        /// <returns>返回判断结果，如果返回<c>false</c>，则表示到达末尾！</returns>
        public bool CanRead => _index < _source.Length;

        /// <summary>
        /// 过滤空格。
        /// </summary>
        public void PassOptionalWhitespace()
        {
            while (CanRead && (Current == ' ' || Current == '\t' || Current == '\r' || Current == '\n'))
            {
                _index++;
            }
        }

        /// <summary>
        /// 读取括号。
        /// </summary>
        public string ReadBlock()
        {
            var sb = new StringBuilder();
            _index++;
            var block = 1;
            while (CanRead)
            {
                switch (Current)
                {
                    case '"':
                    case '\'':
                        sb.Append(ReadQuote(Current));
                        continue;
                    case '(':
                        block++;
                        break;
                    case ')':
                        block--;
                        break;
                }
                if (block == 0)
                {
                    return sb.ToString();
                }
                sb.Append(Current);
                _index++;
            }
            throw new Exception("语法错误，没有遇到“)”块结束符！");
        }

        /// <summary>
        /// 读取引用字符串。
        /// </summary>
        public StringBuilder ReadQuote(char quote)
        {
            /*
                xx == 'sdf{{sdfsdf';
                xx == 'sdf\'sd{{}}fsdf';
                xx == 'sdfsd{{}}fsdf\\';
             */
            var sb = new StringBuilder();
            sb.Append(quote);
            _index++;
            while (CanRead)
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

        /// <summary>
        /// 跳过多少位。
        /// </summary>
        /// <param name="length">长度。</param>
        public void Skip(int length = 1)
        {
            _index += length;
        }

        /// <summary>
        /// 定义空字符。
        /// </summary>
        public const char None = '\0';

        /// <summary>
        /// 当前字符。
        /// </summary>
        public char Current => _source[_index];

        /// <summary>
        /// 下一个字符。
        /// </summary>
        public char Next => _index + 1 < _source.Length ? _source[_index + 1] : None;

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString() => _source.Substring(_index);
    }
}