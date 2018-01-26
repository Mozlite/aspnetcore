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
        /// <returns>返回截取得到的字符串。</returns>
        public static string Substring(this string source, string start, string end = null)
        {
            var index = source.IndexOf(start);
            if (index == -1)
                return null;
            source = source.Substring(index + start.Length);
            if (end != null)
            {
                index = source.IndexOf(end);
                if (index == -1)
                    return null;
                source = source.Substring(0, index);
            }
            return source.Trim();
        }
    }
}