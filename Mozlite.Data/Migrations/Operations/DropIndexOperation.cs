namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 删除索引。
    /// </summary>
    public class DropIndexOperation : MigrationOperation
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Name { get;  set; }

        /// <summary>
        /// 表格。
        /// </summary>
        public virtual string Table { get;  set; }
    }
}
