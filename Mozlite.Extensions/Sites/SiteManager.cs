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
        private readonly IRepository<SiteSettingsAdapter> _sdb;
        private readonly IRepository<SiteDomain> _sddb;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _contextAccessor;

        public SiteManager(IRepository<SiteSettingsAdapter> sdb, IRepository<SiteDomain> sddb, IMemoryCache cache, IHttpContextAccessor contextAccessor)
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
        /// 加载所有网站。
        /// </summary>
        /// <returns>返回所有网站。</returns>
        public IEnumerable<SiteSettingsBase> LoadSites()
        {
            return _sdb.AsQueryable()
                .Select(x => new { x.SettingsId, x.SiteName, x.UpdatedDate })
                .AsEnumerable(reader => new SiteSettingsBase
                {
                    SettingsId = reader.GetInt32(0),
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
        /// 添加域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        public DataResult Delete(SiteDomain domain)
        {
            if (_sddb.Delete(x => x.SiteId == domain.SiteId && x.Domain == domain.Domain))
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
        public SiteDomain GetSite()
        {
            LoadCacheDomains().TryGetValue(_contextAccessor.HttpContext.Request.Host.Host, out var site);
            return site;
        }

        /// <summary>
        /// 获取当前网站域名。
        /// </summary>
        /// <returns>返回当前网站域名实例。</returns>
        public async Task<SiteDomain> GetSiteAsync()
        {
            var sites = await LoadCacheDomainsAsync();
            sites.TryGetValue(_contextAccessor.HttpContext.Request.Host.Host, out var site);
            return site;
        }

        /// <summary>
        /// 加载所有网站。
        /// </summary>
        /// <returns>返回所有网站。</returns>
        public Task<IEnumerable<SiteSettingsBase>> LoadSitesAsync()
        {
            return _sdb.AsQueryable()
                .Select(x => new { x.SettingsId, x.SiteName, x.UpdatedDate })
                .AsEnumerableAsync(reader => new SiteSettingsBase
                {
                    SettingsId = reader.GetInt32(0),
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
        /// 添加域名。
        /// </summary>
        /// <param name="domain">网站域名。</param>
        /// <returns>返回添加结果。</returns>
        public async Task<DataResult> DeleteAsync(SiteDomain domain)
        {
            if (await _sddb.DeleteAsync(x => x.SiteId == domain.SiteId && x.Domain == domain.Domain))
            {
                _cache.Remove(typeof(SiteDomain));
                return DataAction.Deleted;
            }
            return DataAction.DeletedFailured;
        }

        /// <summary>
        /// 保存配置实例。
        /// </summary>
        /// <param name="siteSettings">当前配置实例。</param>
        /// <returns>返回数据结果。</returns>
        public DataResult Save(SiteSettingsBase siteSettings)
        {
            var adapter = new SiteSettingsAdapter();
            adapter.SettingsJSON = JsonConvert.SerializeObject(siteSettings);
            adapter.SettingsId = siteSettings.SettingsId;
            adapter.SiteName = siteSettings.SiteName;
            if (adapter.SettingsId > 0)
                return DataResult.FromResult(_sdb.Update(adapter), DataAction.Updated);
            return DataResult.FromResult(_sdb.Create(adapter), DataAction.Created);
        }

        /// <summary>
        /// 获取当前域名下的网站配置。
        /// </summary>
        /// <typeparam name="TSiteSettings">配置类型。</typeparam>
        /// <returns>返回当前网站配置。</returns>
        public TSiteSettings GetSiteSettings<TSiteSettings>() where TSiteSettings : SiteSettingsBase, new()
        {
            var site = GetSite();
            if (site != null)
                return GetSiteSettings<TSiteSettings>(site.SiteId);
            return null;
        }

        /// <summary>
        /// 获取当前网站配置。
        /// </summary>
        /// <typeparam name="TSiteSettings">配置类型。</typeparam>
        /// <returns>返回当前网站配置。</returns>
        public async Task<TSiteSettings> GetSiteSettingsAsync<TSiteSettings>() where TSiteSettings : SiteSettingsBase, new()
        {
            var site = await GetSiteAsync();
            if (site != null)
                return await GetSiteSettingsAsync<TSiteSettings>(site.SiteId);
            return null;
        }

        /// <summary>
        /// 获取当前域名下的网站配置。
        /// </summary>
        /// <typeparam name="TSiteSettings">配置类型。</typeparam>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回当前网站配置。</returns>
        public TSiteSettings GetSiteSettings<TSiteSettings>(string domain) where TSiteSettings : SiteSettingsBase, new()
        {
            if (LoadCacheDomains().TryGetValue(domain, out var site))
            {
                return GetSiteSettings<TSiteSettings>(site.SiteId);
            }
            return null;
        }

        /// <summary>
        /// 获取当前网站配置。
        /// </summary>
        /// <typeparam name="TSiteSettings">配置类型。</typeparam>
        /// <param name="settingsId">配置ID。</param>
        /// <returns>返回当前网站配置。</returns>
        public TSiteSettings GetSiteSettings<TSiteSettings>(int settingsId) where TSiteSettings : SiteSettingsBase, new()
        {
            var settings = _sdb.Find(settingsId);
            if (settings == null) return null;
            TSiteSettings siteSettings;
            if (string.IsNullOrWhiteSpace(settings.SettingsJSON))
                siteSettings = new TSiteSettings();
            else
                siteSettings = JsonConvert.DeserializeObject<TSiteSettings>(settings.SettingsJSON);
            siteSettings.SiteName = settings.SiteName;
            siteSettings.UpdatedDate = settings.UpdatedDate;
            siteSettings.SettingsId = settingsId;
            return siteSettings;
        }

        /// <summary>
        /// 保存配置实例。
        /// </summary>
        /// <param name="siteSettings">当前配置实例。</param>
        /// <returns>返回数据结果。</returns>
        public async Task<DataResult> SaveAsync(SiteSettingsBase siteSettings)
        {
            var adapter = new SiteSettingsAdapter();
            adapter.SettingsJSON = JsonConvert.SerializeObject(siteSettings);
            adapter.SettingsId = siteSettings.SettingsId;
            adapter.SiteName = siteSettings.SiteName;
            if (adapter.SettingsId > 0)
                return DataResult.FromResult(await _sdb.UpdateAsync(adapter), DataAction.Updated);
            return DataResult.FromResult(await _sdb.CreateAsync(adapter), DataAction.Created);
        }

        /// <summary>
        /// 获取当前域名下的网站配置。
        /// </summary>
        /// <typeparam name="TSiteSettings">配置类型。</typeparam>
        /// <param name="domain">当前域名。</param>
        /// <returns>返回当前网站配置。</returns>
        public async Task<TSiteSettings> GetSiteSettingsAsync<TSiteSettings>(string domain) where TSiteSettings : SiteSettingsBase, new()
        {
            var sites = await LoadCacheDomainsAsync();
            if (sites.TryGetValue(domain, out var site))
            {
                return await GetSiteSettingsAsync<TSiteSettings>(site.SiteId);
            }
            return null;
        }

        /// <summary>
        /// 获取当前网站配置。
        /// </summary>
        /// <typeparam name="TSiteSettings">配置类型。</typeparam>
        /// <param name="settingsId">配置ID。</param>
        /// <returns>返回当前网站配置。</returns>
        public async Task<TSiteSettings> GetSiteSettingsAsync<TSiteSettings>(int settingsId) where TSiteSettings : SiteSettingsBase, new()
        {
            var settings = await _sdb.FindAsync(settingsId);
            if (settings == null) return null;
            TSiteSettings siteSettings;
            if (string.IsNullOrWhiteSpace(settings.SettingsJSON))
                siteSettings = new TSiteSettings();
            else
                siteSettings = JsonConvert.DeserializeObject<TSiteSettings>(settings.SettingsJSON);
            siteSettings.SiteName = settings.SiteName;
            siteSettings.UpdatedDate = settings.UpdatedDate;
            siteSettings.SettingsId = settingsId;
            return siteSettings;
        }
    }
}