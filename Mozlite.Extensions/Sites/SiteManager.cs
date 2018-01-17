using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Newtonsoft.Json;

namespace Mozlite.Extensions.Sites
{
    /// <summary>
    /// 网站管理类。
    /// </summary>
    public class SiteManager : ISiteManager
    {
        private readonly IRepository<SiteAdapter> _sdb;
        private readonly IRepository<SiteDomain> _sddb;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _contextAccessor;

        public SiteManager(IRepository<SiteAdapter> sdb, IRepository<SiteDomain> sddb, IMemoryCache cache, IHttpContextAccessor contextAccessor)
        {
            _sdb = sdb;
            _sddb = sddb;
            _cache = cache;
            _contextAccessor = contextAccessor;
        }

        private IDictionary<string, SiteDomain> LoadCacheDomains()
        {
            return _cache.GetOrCreate(typeof(SiteDomain), ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                return _sddb.Fetch()
                    .OrderByDescending(x => x.Domain.Length)
                    .ToDictionary(x => x.Domain, StringComparer.OrdinalIgnoreCase);
            });
        }

        private async Task<IDictionary<string, SiteDomain>> LoadCacheDomainsAsync()
        {
            return await _cache.GetOrCreateAsync(typeof(SiteDomain), async ctx =>
            {
                ctx.SetAbsoluteExpiration(TimeSpan.FromMinutes(3));
                var domains = await _sddb.FetchAsync();
                return domains.OrderByDescending(x => x.Domain.Length)
                    .ToDictionary(x => x.Domain, StringComparer.OrdinalIgnoreCase);
            });
        }

        /// <summary>
        /// 设置默认。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">域名。</param>
        /// <returns>返回设置结果。</returns>
        public bool SetDefault(int siteId, string domain)
        {
            if (_sddb.BeginTransaction(db =>
            {
                db.Update(x => x.SiteId == siteId, new { IsDefault = false });
                db.Update(x => x.SiteId == siteId && x.Domain == domain, new { IsDefault = true });
                return true;
            }))
            {
                _cache.Remove(typeof(SiteDomain));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置默认。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">域名。</param>
        /// <returns>返回设置结果。</returns>
        public async Task<bool> SetDefaultAsync(int siteId, string domain)
        {
            if (await _sddb.BeginTransactionAsync(async db =>
            {
                await db.UpdateAsync(x => x.SiteId == siteId, new { IsDefault = false });
                await db.UpdateAsync(x => x.SiteId == siteId && x.Domain == domain, new { IsDefault = true });
                return true;
            }))
            {
                _cache.Remove(typeof(SiteDomain));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 禁用域名。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">域名。</param>
        /// <param name="disabled">禁用。</param>
        /// <returns>返回设置结果。</returns>
        public bool SetDisabled(int siteId, string domain, bool disabled = true)
        {
            if (_sddb.Update(x => x.SiteId == siteId && x.Domain == domain, new { Disabled = disabled }))
            {
                _cache.Remove(typeof(SiteDomain));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 禁用域名。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">域名。</param>
        /// <param name="disabled">禁用。</param>
        /// <returns>返回设置结果。</returns>
        public async Task<bool> SetDisabledAsync(int siteId, string domain, bool disabled = true)
        {
            if (await _sddb.UpdateAsync(x => x.SiteId == siteId && x.Domain == domain, new { Disabled = disabled }))
            {
                _cache.Remove(typeof(SiteDomain));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <returns>返回所有网站。</returns>
        public IEnumerable<SiteBase> LoadSites()
        {
            return _sdb.AsQueryable()
                .Select(x => new { x.SiteId, x.SiteName, x.UpdatedDate })
                .AsEnumerable(reader => new SiteBase
                {
                    SiteId = reader.GetInt32(0),
                    SiteName = reader.GetString(1),
                    UpdatedDate = reader.GetFieldValue<DateTimeOffset>(2)
                });
        }

        /// <summary>
        /// 加载所有域名。
        /// </summary>
        /// <returns>返回所有网站域名。</returns>
        public IEnumerable<SiteDomain> LoadDomains()
        {
            return LoadCacheDomains().Values;
        }

        /// <summary>
        /// 判断当前域名是否合法。
        /// </summary>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsValid(string domain)
        {
            if (domain.Equals("localhost", StringComparison.OrdinalIgnoreCase) || domain.Equals("127.0.0.1"))
                return true;
            return LoadCacheDomains().ContainsKey(domain);
        }

        /// <summary>
        /// 添加域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        public DataResult Create(SiteDomain domain)
        {
            if (IsValid(domain.Domain))
                return DataAction.Created;
            if (_sddb.Create(domain))
            {
                _cache.Remove(typeof(SiteDomain));
                return DataAction.Created;
            }
            return DataAction.CreatedFailured;
        }

        /// <summary>
        /// 删除域名。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        public DataResult Delete(int siteId, string domain)
        {
            if (_sddb.Delete(x => x.SiteId == siteId && x.Domain == domain))
            {
                _cache.Remove(typeof(SiteDomain));
                return DataAction.Deleted;
            }
            return DataAction.DeletedFailured;
        }

        /// <summary>
        /// 获取当前网站域名。
        /// </summary>
        /// <returns>返回当前网站域名实例。</returns>
        public SiteDomain GetDomain()
        {
            LoadCacheDomains().TryGetValue(_contextAccessor.HttpContext.Request.Host.Host, out var site);
            return site;
        }

        /// <summary>
        /// 获取当前网站域名。
        /// </summary>
        /// <returns>返回当前网站域名实例。</returns>
        public async Task<SiteDomain> GetDomainAsync()
        {
            var sites = await LoadCacheDomainsAsync();
            sites.TryGetValue(_contextAccessor.HttpContext.Request.Host.Host, out var site);
            return site;
        }

        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <returns>返回所有网站。</returns>
        public Task<IEnumerable<SiteBase>> LoadSitesAsync()
        {
            return _sdb.AsQueryable()
                .Select(x => new { x.SiteId, x.SiteName, x.UpdatedDate })
                .AsEnumerableAsync(reader => new SiteBase
                {
                    SiteId = reader.GetInt32(0),
                    SiteName = reader.GetString(1),
                    UpdatedDate = reader.GetFieldValue<DateTimeOffset>(2)
                });
        }

        /// <summary>
        /// 加载所有域名。
        /// </summary>
        /// <returns>返回所有网站域名。</returns>
        public async Task<IEnumerable<SiteDomain>> LoadDomainsAsync()
        {
            var domains = await LoadCacheDomainsAsync();
            return domains.Values;
        }

        /// <summary>
        /// 判断当前域名是否合法。
        /// </summary>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回判断结果。</returns>
        public async Task<bool> IsValidAsync(string domain)
        {
            if (domain.Equals("localhost", StringComparison.OrdinalIgnoreCase) || domain.Equals("127.0.0.1"))
                return true;
            var domains = await LoadCacheDomainsAsync();
            return domains.ContainsKey(domain);
        }

        /// <summary>
        /// 添加域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        public async Task<DataResult> CreateAsync(SiteDomain domain)
        {
            if (await IsValidAsync(domain.Domain))
                return DataAction.Created;
            if (await _sddb.CreateAsync(domain))
            {
                _cache.Remove(typeof(SiteDomain));
                return DataAction.Created;
            }
            return DataAction.CreatedFailured;
        }

        /// <summary>
        /// 删除域名。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        public async Task<DataResult> DeleteAsync(int siteId, string domain)
        {
            if (await _sddb.DeleteAsync(x => x.SiteId == siteId && x.Domain == domain))
            {
                _cache.Remove(typeof(SiteDomain));
                return DataAction.Deleted;
            }
            return DataAction.DeletedFailured;
        }

        /// <summary>
        /// 保存配置实例。
        /// </summary>
        /// <param name="site">当前配置实例。</param>
        /// <returns>返回数据结果。</returns>
        public DataResult Save(SiteBase site)
        {
            var adapter = new SiteAdapter();
            adapter.SettingValue = JsonConvert.SerializeObject(site);
            adapter.SiteId = site.SiteId;
            adapter.SiteName = site.SiteName;
            if (adapter.SiteId > 0)
                return DataResult.FromResult(_sdb.Update(adapter), DataAction.Updated);
            return DataResult.FromResult(_sdb.Create(adapter), DataAction.Created);
        }

        /// <summary>
        /// 获取当前域名下的网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <returns>返回当前网站信息实例。</returns>
        public TSite GetSite<TSite>() where TSite : SiteBase, new()
        {
            var site = GetDomain();
            if (site != null)
                return GetSite<TSite>(site.SiteId);
            return null;
        }

        /// <summary>
        /// 获取当前网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <returns>返回当前网站信息实例。</returns>
        public async Task<TSite> GetSiteAsync<TSite>() where TSite : SiteBase, new()
        {
            var site = await GetDomainAsync();
            if (site != null)
                return await GetSiteAsync<TSite>(site.SiteId);
            return null;
        }

        /// <summary>
        /// 获取当前域名下的网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回当前网站信息实例。</returns>
        public TSite GetSite<TSite>(string domain) where TSite : SiteBase, new()
        {
            if (LoadCacheDomains().TryGetValue(domain, out var site))
            {
                return GetSite<TSite>(site.SiteId);
            }
            return null;
        }

        /// <summary>
        /// 获取当前网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回当前网站信息实例。</returns>
        public TSite GetSite<TSite>(int siteId) where TSite : SiteBase, new()
        {
            var settings = _sdb.Find(siteId);
            if (settings == null) return null;
            TSite site;
            if (string.IsNullOrWhiteSpace(settings.SettingValue))
                site = new TSite();
            else
                site = JsonConvert.DeserializeObject<TSite>(settings.SettingValue);
            site.SiteName = settings.SiteName;
            site.UpdatedDate = settings.UpdatedDate;
            site.SiteId = siteId;
            return site;
        }

        /// <summary>
        /// 保存配置实例。
        /// </summary>
        /// <param name="site">当前配置实例。</param>
        /// <returns>返回数据结果。</returns>
        public async Task<DataResult> SaveAsync(SiteBase site)
        {
            var adapter = new SiteAdapter();
            adapter.SettingValue = JsonConvert.SerializeObject(site);
            adapter.SiteId = site.SiteId;
            adapter.SiteName = site.SiteName;
            if (adapter.SiteId > 0)
                return DataResult.FromResult(await _sdb.UpdateAsync(adapter), DataAction.Updated);
            return DataResult.FromResult(await _sdb.CreateAsync(adapter), DataAction.Created);
        }

        /// <summary>
        /// 获取当前域名下的网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回当前网站信息实例。</returns>
        public async Task<TSite> GetSiteAsync<TSite>(string domain) where TSite : SiteBase, new()
        {
            var sites = await LoadCacheDomainsAsync();
            if (sites.TryGetValue(domain, out var site))
            {
                return await GetSiteAsync<TSite>(site.SiteId);
            }
            return null;
        }

        /// <summary>
        /// 获取当前网站信息实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回当前网站信息实例。</returns>
        public async Task<TSite> GetSiteAsync<TSite>(int siteId) where TSite : SiteBase, new()
        {
            var settings = await _sdb.FindAsync(siteId);
            if (settings == null) return null;
            TSite site;
            if (string.IsNullOrWhiteSpace(settings.SettingValue))
                site = new TSite();
            else
                site = JsonConvert.DeserializeObject<TSite>(settings.SettingValue);
            site.SiteName = settings.SiteName;
            site.UpdatedDate = settings.UpdatedDate;
            site.SiteId = siteId;
            return site;
        }
    }
}