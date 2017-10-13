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

        /// <summary>
        /// 设置配置得扩展属性。
        /// </summary>
        /// <param name="name">当前属性名称。</param>
        /// <param name="value">当前属性值。</param>
        /// <returns>返回当前扩展构建实例。</returns>
        public OperationBuilder<TOperation> Set(string name, object value)
        {
            Operation[name] = value.ToString();
            return this;
        }
    }
}
