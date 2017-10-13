namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 添加唯一键操作。
    /// </summary>
    public class AddUniqueConstraintOperation : NameTableMigrationOperation, IClusteredOperation
    {
        /// <summary>
        /// 相关列。
        /// </summary>
        public virtual string[] Columns { get;  set; }

        /// <summary>
        /// 是否聚合索引。
        /// </summary>
        public virtual bool IsClustered { get; set; }
    }
}
