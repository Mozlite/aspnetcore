
namespace Mozlite.Data.Migrations.Operations
{
    /// <summary>
    /// 重新开始排序操作。
    /// </summary>
    public class RestartSequenceOperation : MigrationOperation
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
        /// 开始值。
        /// </summary>
        public virtual long StartValue { get; set; } = 1L;
    }
}
