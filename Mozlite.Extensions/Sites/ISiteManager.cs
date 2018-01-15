using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 网站管理接口。
    /// </summary>
    public interface ISiteManager : ISingletonService
    {
        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <returns>返回所有网站。</returns>
        IEnumerable<SiteSettingsBase> LoadSites();

        /// <summary>
        /// 加载所有域名。
        /// </summary>
        /// <returns>返回所有网站域名。</returns>
        IEnumerable<SiteDomain> LoadDomains();

        /// <summary>
        /// 判断当前域名是否合法。
        /// </summary>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回判断结果。</returns>
        bool IsValid(string domain);

        /// <summary>
        /// 添加域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        DataResult Create(SiteDomain domain);

        /// <summary>
        /// 添加域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        DataResult Delete(SiteDomain domain);

        /// <summary>
        /// 获取当前网站域名。
        /// </summary>
        /// <returns>返回当前网站域名实例。</returns>
        SiteDomain GetSite();

        /// <summary>
        /// 获取当前网站域名。
        /// </summary>
        /// <returns>返回当前网站域名实例。</returns>
        Task<SiteDomain> GetSiteAsync();

        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <returns>返回所有网站。</returns>
        Task<IEnumerable<SiteSettingsBase>> LoadSitesAsync();

        /// <summary>
        /// 加载所有域名。
        /// </summary>
        /// <returns>返回所有网站域名。</returns>
        Task<IEnumerable<SiteDomain>> LoadDomainsAsync();

        /// <summary>
        /// 判断当前域名是否合法。
        /// </summary>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> IsValidAsync(string domain);

        /// <summary>
        /// 添加域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        Task<DataResult> CreateAsync(SiteDomain domain);

        /// <summary>
        /// 添加域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        Task<DataResult> DeleteAsync(SiteDomain domain);

        /// <summary>
        /// 保存配置实例。
        /// </summary>
        /// <param name="siteSettings">当前配置实例。</param>
        /// <returns>返回数据结果。</returns>
        DataResult Save(SiteSettingsBase siteSettings);

        /// <summary>
        /// 获取当前域名下的网站配置。
        /// </summary>
        /// <typeparam name="TSiteSettings">配置类型。</typeparam>
        /// <returns>返回当前网站配置。</returns>
        TSiteSettings GetSiteSettings<TSiteSettings>() where TSiteSettings : SiteSettingsBase, new();

        /// <summary>
        /// 获取当前网站配置。
        /// </summary>
        /// <typeparam name="TSiteSettings">配置类型。</typeparam>
        /// <returns>返回当前网站配置。</returns>
        Task<TSiteSettings> GetSiteSettingsAsync<TSiteSettings>() where TSiteSettings : SiteSettingsBase, new();

        /// <summary>
        /// 获取当前域名下的网站配置。
        /// </summary>
        /// <typeparam name="TSiteSettings">配置类型。</typeparam>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回当前网站配置。</returns>
        TSiteSettings GetSiteSettings<TSiteSettings>(string domain) where TSiteSettings : SiteSettingsBase, new();

        /// <summary>
        /// 获取当前网站配置。
        /// </summary>
        /// <typeparam name="TSiteSettings">配置类型。</typeparam>
        /// <param name="settingsId">配置ID。</param>
        /// <returns>返回当前网站配置。</returns>
        TSiteSettings GetSiteSettings<TSiteSettings>(int settingsId) where TSiteSettings : SiteSettingsBase, new();

        /// <summary>
        /// 保存配置实例。
        /// </summary>
        /// <param name="siteSettings">当前配置实例。</param>
        /// <returns>返回数据结果。</returns>
        Task<DataResult> SaveAsync(SiteSettingsBase siteSettings);

        /// <summary>
        /// 获取当前域名下的网站配置。
        /// </summary>
        /// <typeparam name="TSiteSettings">配置类型。</typeparam>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回当前网站配置。</returns>
        Task<TSiteSettings> GetSiteSettingsAsync<TSiteSettings>(string domain) where TSiteSettings : SiteSettingsBase, new();

        /// <summary>
        /// 获取当前网站配置。
        /// </summary>
        /// <typeparam name="TSiteSettings">配置类型。</typeparam>
        /// <param name="settingsId">配置ID。</param>
        /// <returns>返回当前网站配置。</returns>
        Task<TSiteSettings> GetSiteSettingsAsync<TSiteSettings>(int settingsId) where TSiteSettings : SiteSettingsBase, new();
    }
}