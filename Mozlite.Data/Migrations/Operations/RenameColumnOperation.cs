namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 修改列名称。
    /// </summary>
    public class RenameColumnOperation : NameTableMigrationOperation
    {
        /// <summary>
        /// 新名称。
        /// </summary>
        public virtual string NewName { get;  set; }
    }
}
