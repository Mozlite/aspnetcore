namespace Mozlite.Extensions.Searching
{
    /// <summary>
    /// 索引类型。
    /// </summary>
    public enum IndexedType
    {
        /// <summary>
        /// 忽略索引。
        /// </summary>
        Ignore,
        /// <summary>
        /// 等待索引。
        /// </summary>
        Pending,
        /// <summary>
        /// 索引成功。
        /// </summary>
        Completed,
    }
}