using Mozlite.Extensions.Data;

namespace Mozlite.Extensions.Extensions.Data
{
    /// <summary>
    /// 唯一Id对象接口。
    /// </summary>
    /// <typeparam name="TKey">Id类型。</typeparam>
    public interface ISitableObject<TKey> : IIdObject<TKey>, ISitable
    {
    }

    /// <summary>
    /// 唯一Id对象接口。
    /// </summary>
    public interface ISitableObject : ISitableObject<int>
    {
    }
}