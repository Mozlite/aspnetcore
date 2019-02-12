using System.Threading.Tasks;

namespace Mozlite.Extensions.Messages.SMS
{
    /// <summary>
    /// 短信配置管理接口。
    /// </summary>
    public interface ISmsSettingManager : ICachableObjectManager<SmsSettings>, ISingletonService
    {
        /// <summary>
        /// 通过名称查询配置。
        /// </summary>
        /// <param name="name">客户端名称。</param>
        /// <returns>返回配置实例。</returns>
        SmsSettings Find(string name);

        /// <summary>
        /// 通过名称查询配置。
        /// </summary>
        /// <param name="name">客户端名称。</param>
        /// <returns>返回配置实例。</returns>
        Task<SmsSettings> FindAsync(string name);
    }
}