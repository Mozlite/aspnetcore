namespace Mozlite.Extensions
{
    /// <summary>
    /// 唯一Id对象接口。
    /// </summary>
    /// <typeparam name="TKey">Id类型。</typeparam>
    public interface IIdObject<TKey>
    {
        /// <summary>
        /// 获取或设置唯一Id。
        /// </summary>
        TKey Id { get; set; }
    }

    /// <summary>
    /// 唯一Id对象接口。
    /// </summary>
    public interface IIdObject : IIdObject<int>
    {
    }
}