using System.Linq;
using System.Text.RegularExpressions;

namespace Mozlite.Utils
{
    /// <summary>
    /// HTML扩展辅助类。
    /// </summary>
    public static class HtmlHelper
    {
        private static readonly Regex _htmlRegex = new Regex("</*[a-z].*?>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// 移除所有HTML标记。
        /// </summary>
        /// <param name="source">当前代码。</param>
        /// <param name="isBlank">是否移除空格。</param>
        /// <returns>返回移除后的结果。</returns>
        public static string EscapeHtml(this string source, bool isBlank = false)
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
        public static string Decode(this string source)
        {
            source = source.Replace("&quot;", "\"");
            source = source.Replace("&amp;", "&");
            source = source.Replace("&lt;", "<");
            source = source.Replace("&gt;", ">");
            return source;
        }
    }
}