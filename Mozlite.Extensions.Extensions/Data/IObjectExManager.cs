using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Data
{
    /// <summary>
    /// 对象管理接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    /// <typeparam name="TKey">唯一键类型。</typeparam>
    public interface IObjectExManager<TModel, TKey> : IObjectManager<TModel, TKey>
        where TModel : IIdSiteObject<TKey>
    {
        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <returns>返回分页实例列表。</returns>
        new TQuery Load<TQuery>(TQuery query) where TQuery : QueryExBase<TModel>;

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分页实例列表。</returns>
        new Task<TQuery> LoadAsync<TQuery>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : QueryExBase<TModel>;
    }

    /// <summary>
    /// 对象管理接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public interface IObjectExManager<TModel> : IObjectExManager<TModel, int>
        where TModel : IIdSiteObject
    {
    }
}
