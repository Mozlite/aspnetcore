namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 修改表格名称。
    /// </summary>
    public class RenameTableOperation : MigrationOperation
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Table { get;  set; }
        
        /// <summary>
        /// 新名称。
        /// </summary>
        public virtual string NewTable { get;  set; }
    }
}
