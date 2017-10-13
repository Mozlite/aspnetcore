namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 索引名称更改操作。
    /// </summary>
    public class RenameIndexOperation : NameTableMigrationOperation
    {
        /// <summary>
        /// 新名称。
        /// </summary>
        public virtual string NewName { get;  set; }
    }
}
