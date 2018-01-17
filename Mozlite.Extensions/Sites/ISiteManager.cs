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
        /// 设置默认。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">域名。</param>
        /// <returns>返回设置结果。</returns>
        bool SetDefault(int siteId, string domain);

        /// <summary>
        /// 设置默认。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">域名。</param>
        /// <returns>返回设置结果。</returns>
        Task<bool> SetDefaultAsync(int siteId, string domain);

        /// <summary>
        /// 禁用域名。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">域名。</param>
        /// <param name="disabled">禁用。</param>
        /// <returns>返回设置结果。</returns>
        bool SetDisabled(int siteId, string domain, bool disabled = true);

        /// <summary>
        /// 禁用域名。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">域名。</param>
        /// <param name="disabled">禁用。</param>
        /// <returns>返回设置结果。</returns>
        Task<bool> SetDisabledAsync(int siteId, string domain, bool disabled = true);

        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <returns>返回所有网站。</returns>
        IEnumerable<SiteBase> LoadSites();

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
        /// 删除域名。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        DataResult Delete(int siteId, string domain);

        /// <summary>
        /// 获取当前网站域名。
        /// </summary>
        /// <returns>返回当前网站域名实例。</returns>
        SiteDomain GetDomain();

        /// <summary>
        /// 获取当前网站域名。
        /// </summary>
        /// <returns>返回当前网站域名实例。</returns>
        Task<SiteDomain> GetDomainAsync();

        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <returns>返回所有网站。</returns>
        Task<IEnumerable<SiteBase>> LoadSitesAsync();

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
        /// 删除域名。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        Task<DataResult> DeleteAsync(int siteId, string domain);

        /// <summary>
        /// 保存配置实例。
        /// </summary>
        /// <param name="site">当前配置实例。</param>
        /// <returns>返回数据结果。</returns>
        DataResult Save(SiteBase site);

        /// <summary>
        /// 获取当前域名下的网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <returns>返回当前网站信息实例。</returns>
        TSite GetSite<TSite>() where TSite : SiteBase, new();

        /// <summary>
        /// 获取当前网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <returns>返回当前网站信息实例。</returns>
        Task<TSite> GetSiteAsync<TSite>() where TSite : SiteBase, new();

        /// <summary>
        /// 获取当前域名下的网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回当前网站信息实例。</returns>
        TSite GetSite<TSite>(string domain) where TSite : SiteBase, new();

        /// <summary>
        /// 获取当前网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回当前网站信息实例。</returns>
        TSite GetSite<TSite>(int siteId) where TSite : SiteBase, new();

        /// <summary>
        /// 保存配置实例。
        /// </summary>
        /// <param name="site">当前配置实例。</param>
        /// <returns>返回数据结果。</returns>
        Task<DataResult> SaveAsync(SiteBase site);

        /// <summary>
        /// 获取当前域名下的网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回当前网站信息实例。</returns>
        Task<TSite> GetSiteAsync<TSite>(string domain) where TSite : SiteBase, new();

        /// <summary>
        /// 获取当前网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回当前网站信息实例。</returns>
        Task<TSite> GetSiteAsync<TSite>(int siteId) where TSite : SiteBase, new();
    }
}