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
    }

    /// <summary>
    /// 对象管理接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public interface IObjectExManager<TModel> : IObjectManager<TModel, int>
        where TModel : IIdSiteObject
    {
    }
}
