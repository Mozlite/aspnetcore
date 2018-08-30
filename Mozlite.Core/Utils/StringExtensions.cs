using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mozlite.Utils
{
    /// <summary>
    /// 字符串扩展类。
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 截图字符串。
        /// </summary>
        /// <param name="source">原字符串。</param>
        /// <param name="start">开始字符串，结果不包含此字符串。</param>
        /// <param name="end">结束字符串，结果不包含此字符串。</param>
        /// <param name="comparison">对比模式。</param>
        /// <returns>返回截取得到的字符串。</returns>
        public static string Substring(this string source, string start, string end = null, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var index = source.IndexOf(start, comparison);
            if (index == -1)
                return null;
            source = source.Substring(index + start.Length);
            if (end != null)
            {
                index = source.IndexOf(end, comparison);
                if (index == -1)
                    return null;
                source = source.Substring(0, index);
            }
            return source.Trim();
        }

        private static readonly Regex _htmlRegex = new Regex("</*[a-z].*?>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// 移除所有HTML标记。
        /// </summary>
        /// <param name="source">当前代码。</param>
        /// <param name="isBlank">是否移除空格。</param>
        /// <returns>返回移除后的结果。</returns>
        public static string RemoveHtml(this string source, bool isBlank = false)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            source = _htmlRegex.Replace(source, string.Empty).Trim();
            if (isBlank)
                source = source
                    .Replace("&nbsp;", string.Empty)
                    .Replace(" ", string.Empty);
            return source;
        }

        /// <summary>
        /// 分隔HTML标记。
        /// </summary>
        /// <param name="source">源代码。</param>
        /// <returns>返回分隔后的字符串。</returns>
        public static string[] SplitHtml(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            return _htmlRegex.Split(source)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }

        /// <summary>
        /// 解码HTML字符。
        /// </summary>
        /// <param name="source">字符串。</param>
        /// <returns>返回解码后的字符串。</returns>
        public static string DecodeHtml(this string source)
        {
            source = source.Replace("&quot;", "\"");
            source = source.Replace("&amp;", "&");
            source = source.Replace("&lt;", "<");
            source = source.Replace("&gt;", ">");
            return source;
        }

        /// <summary>
        /// 判断当前字符是否为引号：'"','`','\''。
        /// </summary>
        /// <param name="current">当前字符。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsQuote(this char current) => current == '"' || current == '`' || current == '\'';
    }
}