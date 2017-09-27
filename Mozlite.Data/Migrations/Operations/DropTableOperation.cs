namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 删除表格。
    /// </summary>
    public class DropTableOperation : MigrationOperation
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Table { get;  set; }
    }
}
