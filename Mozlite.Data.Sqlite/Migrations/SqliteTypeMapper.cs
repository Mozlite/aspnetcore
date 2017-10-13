using System;
using System.Collections.Generic;
using Mozlite.Data.Migrations;

namespace Mozlite.Data.Sqlite.Migrations
{
    /// <summary>
    /// Sqlite类型匹配实现类。
    /// </summary>
    public class SqliteTypeMapper : TypeMapper
    {
        private const string Text = "TEXT";
        private const string Integer = "INTEGER";
        private const string Real = "REAL";
        private const string Blob = "BLOB";
        private readonly IDictionary<Type, string> _simpleMappings = new Dictionary<Type, string>
        {
            { typeof(int), Integer},
            { typeof(long), Integer},
            { typeof(DateTime), Text },
            { typeof(Guid), Blob },
            { typeof(bool), Integer },
            { typeof(byte), Integer },
            { typeof(double), Real },
            { typeof(DateTimeOffset), Text },
            { typeof(char), Integer },
            { typeof(sbyte), Integer },
            { typeof(ushort), Integer },
            { typeof(uint), Integer },
            { typeof(ulong), Integer },
            { typeof(short), Integer },
            { typeof(float), Real },
            { typeof(decimal), Text },
            { typeof(TimeSpan), Text },
            { typeof(byte[]), Blob }
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

            if (_simpleMappings.TryGetValue(type, out var retType))
                return retType;

            return Text;
        }
    }
}