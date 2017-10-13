namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 迁移数据操作基类。
    /// </summary>
    public abstract class NameTableMigrationOperation : MigrationOperation
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 表格。
        /// </summary>
        public virtual string Table { get; set; }
    }
}