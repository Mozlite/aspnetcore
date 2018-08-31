using System;
using System.Globalization;
using System.Text;
using Mozlite.Data.Internal;

namespace Mozlite.Data.MySql
{
    /// <summary>
    /// SQLServer辅助类。
    /// </summary>
    public class MySqlHelper : SqlHelper
    {
        /// <summary>
        /// 参数化字符串。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>返回参数化的字符串。</returns>
		public override string Parameterized(string name)
		{
            return $"?{name}";
		}

        /// <summary>
        /// 将表格名称或列名称加上安全括弧。
        /// </summary>
        /// <param name="identifier">当前标识字符串。</param>
        /// <returns>返回格式化后的字符串。</returns>
        public override string DelimitIdentifier(string identifier)=> $"`{Check.NotEmpty(identifier, nameof(identifier))}`";

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