
namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 删除排序操作。
    /// </summary>
    public class DropSequenceOperation : MigrationOperation
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Name { get;  set; }

        /// <summary>
        /// 架构。
        /// </summary>
        public virtual string Schema { get;  set; }
    }
}
