#if DEBUG
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mozlite.Utils
{
    /// <summary>
    /// 属性格式化器，用于API调用格式化，快速生成属性名称。
    /// </summary>
    public static class Formatter
    {
        private const string Separator = "--sdf%32$4s#df--";
        private static readonly IDictionary<string, string> _typeNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"int", "int" },
            {"int32", "int" },
            {"long", "long" },
            {"int64", "long" },
            {"short", "short" },
            {"int16", "short" },
            {"string", "string" },
            {"guid", "Guid" },
            {"float", "float" },
            {"double", "double" },
            {"bool", "bool" },
            {"boolean", "bool" },
        };

        /// <summary>
        /// 格式化。
        /// </summary>
        /// <param name="source">源代码。</param>
        /// <param name="separator">分隔符正则表达式。</param>
        /// <param name="format">源代码中格式化：n名称，t类型，s描述。</param>
        /// <param name="ignoreCase">忽略大小写。</param>
        /// <returns>返回属性代码字符串。</returns>
        public static string Format(string source, string separator = "\\s+", string format = "nts", bool ignoreCase = true)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;

            source = source.Trim();
            var regex = new Regex(separator, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            format = format.Trim();
            source = regex.Replace(source, Separator, format.Length - 1);
            var dic = new Dictionary<char, string>();
            for (var i = 0; i < format.Length - 1; i++)
            {
                var index = source.IndexOf(Separator, StringComparison.Ordinal);
                if (index == -1)
                    return null;
                dic[format[i]] = source.Substring(0, index).Trim();
                source = source.Substring(index + Separator.Length);
            }
            dic[format[format.Length - 1]] = source;
            var builder = new StringBuilder();
            if (dic.TryGetValue('s', out var summary))
            {
                builder.AppendLine("/// <summary>");
                builder.Append("/// ").AppendLine(summary);
                builder.AppendLine("/// </summary>");
            }
            builder.Append("public ");
            if (!_typeNames.TryGetValue(dic['t'], out var type))
                type = "string";
            builder.Append(type).Append(" ");
            var name = dic['n'];
            name = char.ToUpper(name[0]) + name.Substring(1);
            builder.Append(name).AppendLine("{get;set;}");
            return builder.ToString();
        }

        /// <summary>
        /// 格式化。
        /// </summary>
        /// <param name="source">源代码。</param>
        /// <param name="separator">分隔符正则表达式。</param>
        /// <param name="format">源代码中格式化：n名称，t类型，s描述。</param>
        /// <param name="ignoreCase">忽略大小写。</param>
        /// <returns>返回属性代码字符串。</returns>
        public static string FormatLines(string source, string separator = "\\s+", string format = "nts", bool ignoreCase = true)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;

            var builder = new StringBuilder();
            foreach (var s in source.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                builder.AppendLine(Format(s, separator, format, ignoreCase));
            }
            return builder.ToString();
        }
    }
}
#endif