namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 索引名称更改操作。
    /// </summary>
    public class RenameIndexOperation : MigrationOperation
    {
        /// <summary>
        /// 原有名称。
        /// </summary>
        public virtual string Name { get;  set; }

        /// <summary>
        /// 新名称。
        /// </summary>
        public virtual string NewName { get;  set; }
        
        /// <summary>
        /// 表格名称。
        /// </summary>
        public virtual string Table { get;  set; }
    }
}
