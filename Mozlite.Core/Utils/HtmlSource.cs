using System;
using System.Text.RegularExpressions;

namespace Mozlite.Utils
{
    /// <summary>
    /// HTML源代码。
    /// </summary>
    public class HtmlSource
    {
        /// <summary>
        /// 空字符串。
        /// </summary>
        public static readonly HtmlSource Empty = new HtmlSource("");

        private string _source;
        /// <summary>
        /// 初始化类<see cref="HtmlSource"/>。
        /// </summary>
        /// <param name="source">HTML代码。</param>
        protected HtmlSource(string source)
        {
            _source = source;
        }

        /// <summary>
        /// 是否为空字符串。
        /// </summary>
        public bool IsNullOrWhiteSpace => string.IsNullOrWhiteSpace(_source);

        /// <summary>
        /// 将字符串隐式转换为<see cref="HtmlSource"/>。
        /// </summary>
        /// <param name="source">HTML源代码。</param>
        /// <returns>返回HTML源码实例对象。</returns>
        public static implicit operator HtmlSource(string source)
        {
            return new HtmlSource(source);
        }

        /// <summary>
        /// 截取HTML源码。
        /// </summary>
        /// <param name="start">开始字符串，结果不包含此字符串。</param>
        /// <param name="end">结束字符串，结果不包含此字符串。</param>
        /// <returns>返回截取的字符串。</returns>
        public HtmlSource Substring(string start, string end)
        {
            var index = _source.IndexOf(start, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                return Empty;
            _source = _source.Substring(index + start.Length);
            index = _source.IndexOf(end, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                return Empty;
            var source = _source.Substring(0, index);
            _source = _source.Substring(index + end.Length);
            return source;
        }

        /// <summary>
        /// 查找并截断字符串，没找到返回null。
        /// </summary>
        /// <param name="end">查找的字符串。</param>
        /// <returns>返回<paramref name="end"/>及之前的所有字符串。</returns>
        public HtmlSource SubIndexOf(string end)
        {
            var index = _source.IndexOf(end, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                return Empty;
            var text = _source.Substring(0, index + end.Length);
            _source = _source.Substring(index + end.Length);
            return text;
        }

        /// <summary>
        /// 移除所有HTML标记。
        /// </summary>
        /// <param name="isBlank">是否移除空格。</param>
        /// <returns>返回移除后的结果。</returns>
        public string EscapeHtml(bool isBlank = false)
        {
            _source = _source.EscapeHtml(isBlank);
            return _source;
        }

        /// <summary>
        /// 移除&amp;nbsp;空格。
        /// </summary>
        /// <returns>返回移除后的结果。</returns>
        public string EscapeBlank()
        {
            if (IsNullOrWhiteSpace)
                return null;
            _source = _source
                .Replace("&nbsp;", string.Empty)
                .Replace(" ", string.Empty);
            return _source;
        }

        private static readonly Regex _numberRegex = new Regex("(\\d+)");
        /// <summary>
        /// 将字符串中的数字转换为整形。
        /// </summary>
        /// <returns>返回整形实例对象。</returns>
        public int? AsRegexInt32() => AsRegexInt32(_source);

        /// <summary>
        /// 将字符串中的数字转换为整形。
        /// </summary>
        /// <returns>返回整形实例对象。</returns>
        public short? AsRegexInt16() => AsRegexInt16(_source);

        /// <summary>
        /// 将字符串中的数字转换为整形。
        /// </summary>
        /// <returns>返回整形实例对象。</returns>
        public long? AsRegexInt64() => AsRegexInt64(_source);

        /// <summary>
        /// 将字符串中的数字转换为整形。
        /// </summary>
        /// <param name="source">源代码。</param>
        /// <returns>返回整形实例对象。</returns>
        public static int? AsRegexInt32(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            var match = _numberRegex.Match(source);
            if (match.Success)
            {
                var value = match.Groups[1].Value?.Trim();
                if (int.TryParse(value, out var intValue))
                    return intValue;
            }
            return null;
        }

        /// <summary>
        /// 将字符串中的数字转换为整形。
        /// </summary>
        /// <param name="source">源代码。</param>
        /// <returns>返回整形实例对象。</returns>
        public static short? AsRegexInt16(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            var match = _numberRegex.Match(source);
            if (match.Success)
            {
                var value = match.Groups[1].Value?.Trim();
                if (short.TryParse(value, out var shortValue))
                    return shortValue;
            }
            return null;
        }

        /// <summary>
        /// 将字符串中的数字转换为整形。
        /// </summary>
        /// <param name="source">源代码。</param>
        /// <returns>返回整形实例对象。</returns>
        public static long? AsRegexInt64(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            var match = _numberRegex.Match(source);
            if (match.Success)
            {
                var value = match.Groups[1].Value?.Trim();
                if (long.TryParse(value, out var longValue))
                    return longValue;
            }
            return null;
        }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return _source;
        }

        /// <summary>
        /// 获取第一个标签的属性值。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <returns>返回第一个标签的属性值。</returns>
        public string GetAttribute(string name)
        {
            var source = _source;
            var index = source.IndexOf('>');
            if (index != -1)
                source = source.Substring(0, index);
            return source.Substring($"{name}=\"", "\"");
        }
    }
}