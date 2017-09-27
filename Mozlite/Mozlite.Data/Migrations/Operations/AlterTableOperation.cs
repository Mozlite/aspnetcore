namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 修改表格。
    /// </summary>
    public class AlterTableOperation : MigrationOperation
    {
        /// <summary>
        /// 表格名称。
        /// </summary>
        public virtual string Table { get;  set; }
    }
}
