using Mozlite.Data.Migrations.Operations;

namespace Mozlite.Data.Migrations.Builders
{
    /// <summary>
    /// 操作构建实例对象。
    /// </summary>
    /// <typeparam name="TOperation">操作类型。</typeparam>
    public class OperationBuilder<TOperation>
        where TOperation : MigrationOperation
    {
        /// <summary>
        /// 初始化类<see cref="OperationBuilder{TOperation}"/>。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        public OperationBuilder(TOperation operation)
        {
            Check.NotNull(operation, nameof(operation));

            Operation = operation;
        }

        /// <summary>
        /// 当前操作实例。
        /// </summary>
        protected virtual TOperation Operation { get; }
    }
}
