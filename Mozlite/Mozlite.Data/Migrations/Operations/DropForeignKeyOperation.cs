namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 删除外键操作。
    /// </summary>
    public class DropForeignKeyOperation : MigrationOperation
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
