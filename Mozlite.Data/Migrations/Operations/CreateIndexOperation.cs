namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 新建索引操作。
    /// </summary>
    public class CreateIndexOperation : NameTableMigrationOperation, IClusteredOperation
    {
        /// <summary>
        /// 是否唯一。
        /// </summary>
        public virtual bool IsUnique { get; set; }

        /// <summary>
        /// 是否聚合索引。
        /// </summary>
        public virtual bool IsClustered { get; set; }

        /// <summary>
        /// 相关列。
        /// </summary>
        public virtual string[] Columns { get;  set; }
    }
}
