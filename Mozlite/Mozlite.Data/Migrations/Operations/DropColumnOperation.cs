namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 删除列。
    /// </summary>
    public class DropColumnOperation : MigrationOperation
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
