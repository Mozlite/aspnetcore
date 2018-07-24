using System.Threading.Tasks;

namespace Mozlite.Extensions.Storages.Configuration
{
    /// <summary>
    /// JSON配置数据管理接口。
    /// </summary>
    public interface IConfigurationDataManager : ISingletonService
    {
        /// <summary>
        /// 加载配置。   
        /// </summary>
        /// <typeparam name="TConfiguration">配置类型。</typeparam>
        /// <param name="name">名称，不包含文件扩展名。</param>
        /// <param name="minutes">缓存分钟数。</param>
        /// <returns>返回配置实例。</returns>
        TConfiguration LoadConfiguration<TConfiguration>(string name, int minutes = 3);

        /// <summary>
        /// 加载配置。   
        /// </summary>
        /// <typeparam name="TConfiguration">配置类型。</typeparam>
        /// <param name="name">名称，不包含文件扩展名。</param>
        /// <param name="minutes">缓存分钟数。</param>
        /// <returns>返回配置实例。</returns>
        Task<TConfiguration> LoadConfigurationAsync<TConfiguration>(string name, int minutes = 3);

        /// <summary>
        /// 保存配置。
        /// </summary>
        /// <param name="name">名称，不包含文件扩展名。</param>
        /// <param name="configuration">配置实例。</param>
        void SaveConfiguration(string name, object configuration);

        /// <summary>
        /// 保存配置。
        /// </summary>
        /// <param name="name">名称，不包含文件扩展名。</param>
        /// <param name="configuration">配置实例。</param>
        Task SaveConfigurationAsync(string name, object configuration);
    }
}