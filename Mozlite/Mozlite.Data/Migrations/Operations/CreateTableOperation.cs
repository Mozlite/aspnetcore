using System.Collections.Generic;

namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 新建表格。
    /// </summary>
    public class CreateTableOperation : MigrationOperation
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Table { get;  set; }

        /// <summary>
        /// 主键。
        /// </summary>
        public virtual AddPrimaryKeyOperation PrimaryKey { get;  set; }

        /// <summary>
        /// 添加的列。
        /// </summary>
        public virtual List<AddColumnOperation> Columns { get; } = new List<AddColumnOperation>();

        /// <summary>
        /// 添加的外键。
        /// </summary>
        public virtual List<AddForeignKeyOperation> ForeignKeys { get; } = new List<AddForeignKeyOperation>();

        /// <summary>
        /// 唯一约束。
        /// </summary>
        public virtual List<AddUniqueConstraintOperation> UniqueConstraints { get; } = new List<AddUniqueConstraintOperation>();
    }
}
