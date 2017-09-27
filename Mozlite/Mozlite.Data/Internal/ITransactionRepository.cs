namespace Mozlite.Data.Internal
{
    /// <summary>
    /// 数据库事务操作接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public interface ITransactionRepository<TModel> : IRepositoryBase<TModel>
    {
        /// <summary>
        /// 获取其他模型表格操作实例。
        /// </summary>
        /// <typeparam name="TOther">其他模型类型。</typeparam>
        /// <returns>返回当前事务的模型数据库操作实例。</returns>
        ITransactionRepository<TOther> As<TOther>();
    }
}