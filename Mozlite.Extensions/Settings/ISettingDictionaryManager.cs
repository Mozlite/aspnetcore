using Mozlite.Extensions.Groups;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Settings
{
    /// <summary>
    /// 字典管理接口。
    /// </summary>
    public interface ISettingDictionaryManager : IGroupManager<SettingDictionary>, ISingletonService
    {
        /// <summary>
        /// 通过路径获取字典值。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>返回字典值。</returns>
        string GetSettings(string path);

        /// <summary>
        /// 通过路径获取字典值。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>返回字典值。</returns>
        Task<string> GetSettingsAsync(string path);

        /// <summary>
        /// 通过路径获取字典值。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>返回字典值。</returns>
        string GetOrAddSettings(string path);

        /// <summary>
        /// 通过路径获取字典值。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>返回字典值。</returns>
        Task<string> GetOrAddSettingsAsync(string path);
    }
}