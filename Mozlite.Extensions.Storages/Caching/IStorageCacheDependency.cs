namespace Mozlite.Extensions.Storages.Caching
{
    /// <summary>
    /// 存储缓存依赖项接口。
    /// </summary>
    public interface IStorageCacheDependency
    {
        /// <summary>
        /// 是否相等。
        /// </summary>
        /// <param name="dependency">缓存依赖项的值。</param>
        /// <returns>返回判断结果。</returns>
        bool IsEqual(string dependency);

        /// <summary>
        /// 是否相等。
        /// </summary>
        /// <param name="dependency">缓存依赖项。</param>
        /// <returns>返回判断结果。</returns>
        bool IsEqual(IStorageCacheDependency dependency);
    }
}