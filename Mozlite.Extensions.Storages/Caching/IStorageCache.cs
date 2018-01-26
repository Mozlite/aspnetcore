using System;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Storages.Caching
{
    /// <summary>
    /// 存储缓存接口。
    /// </summary>
    public interface IStorageCache : ISingletonService
    {
        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        string GetOrCreate(object key, Func<IStorageContext, string> action);

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="dependency">缓存依赖项。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        string GetOrCreate(object key, IStorageCacheDependency dependency, Func<IStorageContext, string> action);

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        Task<string> GetOrCreateAsync(object key, Func<IStorageContext, Task<string>> action);

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="dependency">缓存依赖项。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        Task<string> GetOrCreateAsync(object key, IStorageCacheDependency dependency, Func<IStorageContext, Task<string>> action);

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <typeparam name="TCache">当前缓存对象类型。</typeparam>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        TCache GetOrCreate<TCache>(object key, Func<IStorageContext, TCache> action);

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <typeparam name="TCache">当前缓存对象类型。</typeparam>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="dependency">缓存依赖项。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        TCache GetOrCreate<TCache>(object key, IStorageCacheDependency dependency, Func<IStorageContext, TCache> action);

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <typeparam name="TCache">当前缓存对象类型。</typeparam>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        Task<TCache> GetOrCreateAsync<TCache>(object key, Func<IStorageContext, Task<TCache>> action);

        /// <summary>
        /// 获取或设置缓存对象。
        /// </summary>
        /// <typeparam name="TCache">当前缓存对象类型。</typeparam>
        /// <param name="key">缓存唯一键。</param>
        /// <param name="dependency">缓存依赖项。</param>
        /// <param name="action">获取和配置缓存实例。</param>
        /// <returns>返回当前缓存对象。</returns>
        Task<TCache> GetOrCreateAsync<TCache>(object key, IStorageCacheDependency dependency, Func<IStorageContext, Task<TCache>> action);

        /// <summary>
        /// 移除缓存。
        /// </summary>
        /// <param name="key">缓存唯一键。</param>
        void Remove(object key);

        /// <summary>
        /// 移除缓存。
        /// </summary>
        /// <param name="key">缓存唯一键。</param>
        Task RemoveAsync(object key);
    }
}