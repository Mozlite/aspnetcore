
namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 排序操作。
    /// </summary>
    public class SequenceOperation : MigrationOperation
    {
        /// <summary>
        /// 增量。
        /// </summary>
        public virtual int IncrementBy { get; set; } = 1;

        /// <summary>
        /// 最大值。
        /// </summary>
        public virtual long? MaxValue { get;  set; }

        /// <summary>
        /// 最小值。
        /// </summary>
        public virtual long? MinValue { get;  set; }

        /// <summary>
        /// 是否循环。
        /// </summary>
        public virtual bool IsCyclic { get; set; }
    }
}
