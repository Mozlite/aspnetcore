namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 添加主键。
    /// </summary>
    public class AddPrimaryKeyOperation : MigrationOperation
    {
        /// <summary>
        /// 表格。
        /// </summary>
        public virtual string Table { get;  set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Name { get;  set; }

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
