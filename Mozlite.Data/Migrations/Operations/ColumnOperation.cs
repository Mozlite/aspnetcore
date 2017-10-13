using System;

namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 列操作相关实例。
    /// </summary>
    public class ColumnOperation : NameTableMigrationOperation
    {
        /// <summary>
        /// 是否为Unicode字符集。
        /// </summary>
        public virtual bool? IsUnicode { get;  set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public virtual Type ClrType { get;  set; }

        /// <summary>
        /// 列类型。
        /// </summary>
        public virtual string ColumnType { get;  set; }

        /// <summary>
        /// 大小。
        /// </summary>
        public virtual int? MaxLength { get;  set; }

        /// <summary>
        /// 是否为行版本。
        /// </summary>
        public virtual bool IsRowVersion { get; set; }

        /// <summary>
        /// 是否自增长。
        /// </summary>
        public virtual bool IsIdentity { get; set; }

        /// <summary>
        /// 是否可空。
        /// </summary>
        public virtual bool? IsNullable { get; set; }

        /// <summary>
        /// 默认值。
        /// </summary>
        public virtual object DefaultValue { get;  set; }

        /// <summary>
        /// 默认SQL字符串。
        /// </summary>
        public virtual string DefaultValueSql { get;  set; }

        /// <summary>
        /// 计算列的值字符串。
        /// </summary>
        public virtual string ComputedColumnSql { get;  set; }
    }
}
