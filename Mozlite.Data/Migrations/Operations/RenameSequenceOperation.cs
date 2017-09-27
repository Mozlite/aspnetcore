
namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 修改排序名称。
    /// </summary>
    public class RenameSequenceOperation : MigrationOperation
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Name { get;  set; }

        /// <summary>
        /// 架构。
        /// </summary>
        public virtual string Schema { get;  set; }

        /// <summary>
        /// 新名称。
        /// </summary>
        public virtual string NewName { get;  set; }

        /// <summary>
        /// 新架构。
        /// </summary>
        public virtual string NewSchema { get;  set; }
    }
}
