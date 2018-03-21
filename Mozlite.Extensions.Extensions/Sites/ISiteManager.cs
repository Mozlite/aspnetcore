using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mozlite.Data.Internal;
using Mozlite.Extensions.Data;

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
        /// <param name="domain">网站域名。</param>
        /// <returns>返回当前网站域名实例。</returns>
        SiteDomain GetDomain(string domain);

        /// <summary>
        /// 获取当前网站域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回当前网站域名实例。</returns>
        Task<SiteDomain> GetDomainAsync(string domain);

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
        /// 获取网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="key">网站唯一键。</param>
        /// <returns>返回当前网站信息实例。</returns>
        TSite GetSiteByKey<TSite>(string key) where TSite : SiteBase, new();

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
        /// 获取网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="key">网站唯一键。</param>
        /// <returns>返回当前网站信息实例。</returns>
        Task<TSite> GetSiteByKeyAsync<TSite>(string key) where TSite : SiteBase, new();

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

        /// <summary>
        /// 判断是否已经存在，唯一键重复。
        /// </summary>
        /// <param name="site">当前网站实例。</param>
        /// <returns>返回判断结果。</returns>
        bool IsDuplicated(SiteBase site);

        /// <summary>
        /// 判断是否已经存在，唯一键重复。
        /// </summary>
        /// <param name="site">当前网站实例。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> IsDuplicatedAsync(SiteBase site);

        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <returns>返回所有网站。</returns>
        IEnumerable<TSite> LoadSites<TSite>() where TSite : SiteBase, new();

        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <returns>返回所有网站。</returns>
        Task<IEnumerable<TSite>> LoadSitesAsync<TSite>() where TSite : SiteBase, new();

        /// <summary>
        /// 删除网站。
        /// </summary>
        /// <param name="siteId">网站ID。</param>
        /// <returns>返回删除结果。</returns>
        DataResult Delete(int siteId);

        /// <summary>
        /// 删除网站。
        /// </summary>
        /// <param name="siteId">网站ID。</param>
        /// <returns>返回删除结果。</returns>
        Task<DataResult> DeleteAsync(int siteId);

        /// <summary>
        /// 获取默认网站域名。
        /// </summary>
        /// <param name="siteId">网站ID。</param>
        /// <returns>返回默认网站域名。</returns>
        SiteDomain GetDomain(int siteId);

        /// <summary>
        /// 获取默认网站域名。
        /// </summary>
        /// <param name="siteId">网站ID。</param>
        /// <returns>返回默认网站域名。</returns>
        Task<SiteDomain> GetDomainAsync(int siteId);

        /// <summary>
        /// 初始化整个站的方法。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="executor">数据库事务操作执行方法。</param>
        /// <param name="site">网站实例。</param>
        /// <param name="domain">域名实例。</param>
        /// <returns>返回安装结果。</returns>
        bool Install<TSite>(Func<IDbTransactionContext<SiteDomain>, bool> executor, TSite site, SiteDomain domain)
            where TSite : SiteBase;

        /// <summary>
        /// 初始化整个站的方法。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="executor">数据库事务操作执行方法。</param>
        /// <param name="site">网站实例。</param>
        /// <param name="domain">域名实例。</param>
        /// <returns>返回安装结果。</returns>
        Task<bool> InstallAsync<TSite>(Func<IDbTransactionContext<SiteDomain>, Task<bool>> executor, TSite site, SiteDomain domain)
            where TSite : SiteBase;

        /// <summary>
        /// 获取管理网站实例。
        /// </summary>
        /// <typeparam name="TSite">网站实例。</typeparam>
        /// <returns>返回网站实例对象。</returns>
        Task<TSite> GetAdministratorAsync<TSite>() where TSite : SiteBase, new();

        /// <summary>
        /// 获取管理网站实例。
        /// </summary>
        /// <typeparam name="TSite">网站实例。</typeparam>
        /// <returns>返回网站实例对象。</returns>
        TSite GetAdministrator<TSite>() where TSite : SiteBase, new();
    }
}