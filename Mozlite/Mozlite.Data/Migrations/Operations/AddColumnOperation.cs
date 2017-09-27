namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 添加列。
    /// </summary>
    public class AddColumnOperation : ColumnOperation
    {
        /// <summary>
        /// 列名称。
        /// </summary>
        public virtual string Name { get;  set; }
        
        /// <summary>
        /// 表格。
        /// </summary>
        public virtual string Table { get;  set; }
    }
}
