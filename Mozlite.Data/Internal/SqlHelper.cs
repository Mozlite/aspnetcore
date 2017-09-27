using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace Mozlite.Data.Internal
{
    /// <summary>
    /// SQL辅助类基类。
    /// </summary>
    public abstract class SqlHelper : ISqlHelper
    {
        /// <summary>
        /// 语句结束符。
        /// </summary>
        public virtual string StatementTerminator => ";";

        /// <summary>
        /// 参数化字符串。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>返回参数化的字符串。</returns>
        public virtual string Parameterized(string name)
        {
            return "@" + name;
        }
        
        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="literal">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        public string EscapeLiteral(object literal)
        {
            if (literal == null)
                return "NULL";
            return GenerateLiteralValue((dynamic)literal);
        }

        /// <summary>
        /// 将字符串的“'”替换为“''”。
        /// </summary>
        /// <param name="identifier">当前字符串。</param>
        /// <returns>返回替换后的结果。</returns>
        public string EscapeIdentifier(string identifier)
        {
            if (identifier == null)
                return "NULL";
            return identifier.Replace("'", "''");
        }
        
        /// <summary>
        /// 将表格名称或列名称加上安全括弧。
        /// </summary>
        /// <param name="identifier">当前标识字符串。</param>
        /// <returns>返回格式化后的字符串。</returns>
        public virtual string DelimitIdentifier(string identifier) => $"[{Check.NotEmpty(identifier, nameof(identifier))}]";
        
        /// <summary>
        /// 将表格名称或列名称加上安全括弧。
        /// </summary>
        /// <param name="name">当前标识字符串。</param>
        /// <param name="schema">架构名称。</param>
        /// <returns>返回格式化后的字符串。</returns>
        public virtual string DelimitIdentifier(string name, string schema)
            => (!string.IsNullOrEmpty(schema)
                   ? DelimitIdentifier(schema) + "."
                   : string.Empty)
               + DelimitIdentifier(Check.NotEmpty(name, nameof(name)));
        
        private const string DecimalFormatConst = "0.0###########################";
        private const string DateTimeFormatConst = @"yyyy-MM-dd HH\:mm\:ss.fffffff";
        private const string DateTimeOffsetFormatConst = @"yyyy-MM-dd HH\:mm\:ss.fffffffzzz";

        /// <summary>
        /// float格式。
        /// </summary>
        protected virtual string FloatingPointFormatString => "{0}E0";

        /// <summary>
        /// decimal格式.
        /// </summary>
        protected virtual string DecimalFormat => DecimalFormatConst;

        /// <summary>
        /// 日期格式
        /// </summary>
        protected virtual string DateTimeFormat => DateTimeFormatConst;

        /// <summary>
        /// DateTimeOffset格式。
        /// </summary>
        protected virtual string DateTimeOffsetFormat => DateTimeOffsetFormatConst;

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(string value)
            => $"'{Check.NotNull(value, nameof(value)).Replace("'", "''")}'";

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(int value)
            => value.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(short value)
            => value.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(long value)
            => value.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(byte value)
            => value.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(decimal value)
            => value.ToString(DecimalFormat, CultureInfo.InvariantCulture);

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(double value)
            => string.Format(CultureInfo.InvariantCulture, FloatingPointFormatString, value);

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(float value)
            => string.Format(CultureInfo.InvariantCulture, FloatingPointFormatString, value);

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(bool value)
            => value ? "1" : "0";

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(char value)
            => string.Format(CultureInfo.InvariantCulture, "'{0}'", value);

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue( object value)
            => GenerateLiteralValue(value.ToString());

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue( byte[] value)
        {
            Check.NotNull(value, nameof(value));
            var builder = new StringBuilder();
            builder.Append("X'");
            foreach (var @byte in value)
            {
                builder.Append(@byte.ToString("X2", CultureInfo.InvariantCulture));
            }
            builder.Append("'");
            return builder.ToString();
        }

        private readonly Dictionary<DbType, string> _dbTypeNameMapping = new Dictionary<DbType, string>
        {
            { DbType.Byte, "tinyint" },
            { DbType.Decimal, "decimal" },
            { DbType.Double, "float" },
            { DbType.Int16, "smallint" },
            { DbType.Int32, "int" },
            { DbType.Int64, "bigint" },
            { DbType.String, "nvarchar" },
            { DbType.Date, "date" }
        };

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(DbType value)
            => _dbTypeNameMapping[value];

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue( Enum value)
            => string.Format(CultureInfo.InvariantCulture, "{0:d}", Check.NotNull(value, nameof(value)));

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(Guid value)
            => string.Format(CultureInfo.InvariantCulture, "'{0}'", value);

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(DateTime value)
            => $"TIMESTAMP '{value.ToString(DateTimeFormat, CultureInfo.InvariantCulture)}'";

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(DateTimeOffset value)
            => $"TIMESTAMP '{value.ToString(DateTimeOffsetFormat, CultureInfo.InvariantCulture)}'";

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(TimeSpan value)
            => string.Format(CultureInfo.InvariantCulture, "'{0}'", value);

        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="value">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        protected virtual string GenerateLiteralValue(Type value)
            => GenerateLiteralValue(value.DisplayName());
    }
}