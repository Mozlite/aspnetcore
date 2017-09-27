using System;
using System.Globalization;
using System.Text;
using Mozlite.Data.Internal;

namespace Mozlite.Data.SqlServer
{
    /// <summary>
    /// SQLServer辅助类。
    /// </summary>
    public class SqlServerHelper : SqlHelper
    {
        private const string DateTimeFormatConst = "yyyy-MM-ddTHH:mm:ss.fffK";
        private const string DateTimeOffsetFormatConst = "yyyy-MM-ddTHH:mm:ss.fffzzz";

        /// <summary>
        /// 日期格式
        /// </summary>
        protected override string DateTimeFormat => DateTimeFormatConst;

        /// <summary>
        /// DateTimeOffset格式。
        /// </summary>
        protected override string DateTimeOffsetFormat => DateTimeOffsetFormatConst;

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected override string GenerateLiteralValue(byte[] value)
        {
            Check.NotNull(value, nameof(value));
            var builder = new StringBuilder();
            builder.Append("0x");
            foreach (var @byte in value)
            {
                builder.Append(@byte.ToString("X2", CultureInfo.InvariantCulture));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected override string GenerateLiteralValue(string value)
            => $"N'{Check.NotNull(value, nameof(value)).Replace("'", "''")}'";

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected override string GenerateLiteralValue(DateTime value)
            => $"'{value.ToString(DateTimeFormat, CultureInfo.InvariantCulture)}'";

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected override string GenerateLiteralValue(DateTimeOffset value)
            => $"'{value.ToString(DateTimeOffsetFormat, CultureInfo.InvariantCulture)}'";
    }
}