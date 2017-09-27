using Mozlite.Data.Migrations.Operations;

namespace Mozlite.Data.Migrations.Builders
{
    /// <summary>
    /// 修改操作构建实例。
    /// </summary>
    /// <typeparam name="TOperation">操作类型。</typeparam>
    public class AlterOperationBuilder<TOperation> : OperationBuilder<TOperation>
        where TOperation : MigrationOperation
    {
        /// <summary>
        /// 初始化类<see cref="AlterOperationBuilder{TOperation}"/>。
        /// </summary>
        /// <param name="operation">操作实例对象。</param>
        public AlterOperationBuilder( TOperation operation)
            : base(operation)
        {
        }
    }
}
