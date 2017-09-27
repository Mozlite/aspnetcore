namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 修改排序操作。
    /// </summary>
    public class AlterSequenceOperation : SequenceOperation
    {
        /// <summary>
        /// 架构。
        /// </summary>
        public virtual string Schema { get;  set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Name { get;  set; }
    }
}
