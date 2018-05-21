using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 核心扩展类。
    /// </summary>
    public static class CoreExtensions
    {
        /// <summary>
        /// 设置默认缓存时间，3分钟。
        /// </summary>
        /// <param name="cache">缓存实体接口。</param>
        /// <returns>返回缓存实体接口。</returns>
        public static ICacheEntry SetDefaultAbsoluteExpiration(this ICacheEntry cache)
        {
            return cache.SetAbsoluteExpiration(Cores.DefaultCacheExpiration);
        }

        /// <summary>
        /// 添加数据加密密钥服务。
        /// </summary>
        /// <param name="services">服务器集合。</param>
        /// <param name="directory">存储文件夹。</param>
        /// <returns>返回服务器接口集合。</returns>
        public static IServiceCollection AddMozliteDataProtection(this IServiceCollection services, string directory = "../storages/keys")
        {
            DirectoryInfo info;
            try
            {
                info = new DirectoryInfo(directory);
            }
            catch
            {
                directory = Path.Combine(Directory.GetCurrentDirectory(), directory);
                info = new DirectoryInfo(directory);
            }
            
            services.AddDataProtection()
                .PersistKeysToFileSystem(info)
                .ProtectKeysWithDpapi();
            return services;
        }
    }
}