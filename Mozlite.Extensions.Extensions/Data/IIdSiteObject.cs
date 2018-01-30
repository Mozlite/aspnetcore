namespace Mozlite.Extensions.Data
{
    /// <summary>
    /// 唯一Id对象接口。
    /// </summary>
    /// <typeparam name="TKey">Id类型。</typeparam>
    public interface IIdSiteObject<TKey> : IIdObject<TKey>, ISitable
    {
    }

    /// <summary>
    /// 唯一Id对象接口。
    /// </summary>
    public interface IIdSiteObject : IIdSiteObject<int>
    {
    }
}