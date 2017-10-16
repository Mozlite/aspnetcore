using System;
using System.Linq;
using Microsoft.AspNetCore.Html;

namespace Mozlite.Mvc
{
    /// <summary>
    /// 字符串扩展类。
    /// </summary>
    public static class StringExtensions
    {
        private static readonly char[] _seperators = " \t,/|\\、；：，".ToCharArray();
        /// <summary>
        /// 将关键词字符串按照分隔符拆封，然后格式化后并组合返回。
        /// </summary>
        /// <param name="keywords">关键词。</param>
        /// <param name="format">格式化方法。</param>
        /// <param name="count">返回的项目数量。</param>
        /// <param name="seperator">拼接分隔符。</param>
        /// <param name="end">如果原项目数量超过<paramref name="count"/>附加的字符串，比如：等。</param>
        /// <returns>返回组合后的HTML字符串。</returns>
        public static IHtmlContent JoinSplit(this string keywords, Func<string, string> format, int count = 0, string seperator = null, string end = "等")
        {
            if (string.IsNullOrWhiteSpace(keywords))
                return null;
            var words = keywords.Split(_seperators, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .ToList();
            if (count > 0 && words.Count > count)
                words = words.GetRange(0, count);
            else
                end = null;
            return new HtmlString(string.Join(seperator, words.Select(format)) + end);
        }

        /// <summary>
        /// 将关键词字符串按照分隔符拆封。
        /// </summary>
        /// <param name="keywords">关键词。</param>
        /// <param name="seperator">拼接分隔符。</param>
        /// <returns>返回组合后的字符串。</returns>
        public static string JoinSplit(this string keywords, string seperator = ",")
        {
            if (string.IsNullOrWhiteSpace(keywords))
                return null;
            var words = keywords.Split(_seperators, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .ToList();
            return string.Join(seperator, words);
        }

        /// <summary>
        /// 将参数过滤空字符串或空对象，并拆分和拼接字符串。
        /// </summary>
        /// <param name="keywords">关键词列表。</param>
        /// <param name="seperator">拼接分隔符。</param>
        /// <returns>返回拼接后的字符串。</returns>
        public static string JoinSplit(this object[] keywords, string seperator)
        {
            if (keywords == null || keywords.Length == 0)
                return null;
            var words = keywords.Where(arg => arg != null)
                .Select(arg => arg.ToString().Replace(" ", string.Empty))
                .Where(arg => !string.IsNullOrWhiteSpace(arg))
                .SelectMany(arg => arg.Split(_seperators, StringSplitOptions.RemoveEmptyEntries));
            return string.Join(seperator, words);
        }

        /// <summary>
        /// 将关键词分割。
        /// </summary>
        /// <param name="keywords">关键词集合。</param>
        /// <returns>返回分割后的关键词列表。</returns>
        public static string[] SplitString(this string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
                return null;
            return keywords.Split(_seperators, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
        }
    }
}