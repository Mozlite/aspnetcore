using System;
using System.Collections.Generic;
using Mozlite.Data.Migrations;
using Mozlite.Data.SqlServer.Properties;

namespace Mozlite.Data.SqlServer.Migrations
{
    /// <summary>
    /// SQLServer类型匹配实现类。
    /// </summary>
    public class SqlServerTypeMapper : TypeMapper
    {
        private readonly IDictionary<Type, string> _simpleMappings
                   = new Dictionary<Type, string>
                   {
                    { typeof(int), "int"},
                    { typeof(long), "bigint"},
                    { typeof(DateTime), "datetime2" },
                    { typeof(Guid), "uniqueidentifier" },
                    { typeof(bool), "bit" },
                    { typeof(byte), "tinyint" },
                    { typeof(double), "float" },
                    { typeof(DateTimeOffset), "datetimeoffset" },
                    { typeof(char), "int" },
                    { typeof(sbyte), "smallint" },
                    { typeof(ushort), "int" },
                    { typeof(uint), "bigint" },
                    { typeof(ulong), "numeric(20, 0)" },
                    { typeof(short), "smallint" },
                    { typeof(float), "real" },
                    { typeof(decimal), "decimal" },
                    { typeof(TimeSpan), "time" },
                    { typeof(byte[]), "varbinary" }
                   };
        /// <summary>
        /// 获取数据类型。
        /// </summary>
        /// <param name="type">当前类型实例。</param>
        /// <param name="size">大小。</param>
        /// <param name="rowVersion">是否为RowVersion。</param>
        /// <param name="unicode">是否为Unicode字符集。</param>
        /// <returns>返回匹配的数据类型。</returns>
        public override string GetMapping(Type type, int? size = null, bool rowVersion = false, bool? unicode = null)
        {
            Check.NotNull(type, nameof(type));
            type = type.UnwrapNullableType().UnwrapEnumType();

            if (type == typeof(string))
                return size > 0 ? $"nvarchar({size})" : "nvarchar(max)";
            if (type == typeof(byte[]))
                return size > 0 ? $"varbinary({size})" : "varbinary(max)";
            string retType;
            if (_simpleMappings.TryGetValue(type, out retType))
                return retType;

            throw new NotSupportedException(string.Format(Resources.UnsupportedType, type.DisplayName()));
        }
    }
}