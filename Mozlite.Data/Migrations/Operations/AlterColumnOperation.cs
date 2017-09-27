namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 修改列。
    /// </summary>
    public class AlterColumnOperation : ColumnOperation
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
