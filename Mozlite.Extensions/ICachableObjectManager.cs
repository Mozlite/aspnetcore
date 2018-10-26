namespace Mozlite.Extensions
{
    /// <summary>
    /// 缓存对象管理接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    /// <typeparam name="TKey">模型主键类型。</typeparam>
    public interface ICachableObjectManager<TModel, TKey> : IObjectManager<TModel, TKey>
        where TModel : IIdObject<TKey>
    {
        /// <summary>
        /// 刷新缓存。
        /// </summary>
        void Refresh();
    }

    /// <summary>
    /// 缓存对象管理接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public interface ICachableObjectManager<TModel> : ICachableObjectManager<TModel, int> where TModel : IIdObject
    {
    }
}