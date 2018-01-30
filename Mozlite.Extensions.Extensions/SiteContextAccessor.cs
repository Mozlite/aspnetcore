using System;
using Microsoft.AspNetCore.Http;
using Mozlite.Extensions.Properties;
using Mozlite.Extensions.Sites;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 实现当前网站上下文访问实例。
    /// </summary>
    /// <typeparam name="TSite">网站类型。</typeparam>
    /// <typeparam name="TSiteContext">网站上下文。</typeparam>
    public class SiteContextAccessor<TSite, TSiteContext> : ISiteContextAccessor<TSite, TSiteContext>
        where TSite : SiteBase, new()
        where TSiteContext : SiteContextBase<TSite>, new()
    {
        private readonly ISiteManager _siteManager;
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// 初始化类<see cref="SiteContextAccessor{TSite, TSiteContext}"/>。
        /// </summary>
        /// <param name="siteManager">网站管理接口。</param>
        /// <param name="contextAccessor">HTTP访问上下文。</param>
        public SiteContextAccessor(ISiteManager siteManager, IHttpContextAccessor contextAccessor)
        {
            _siteManager = siteManager;
            _contextAccessor = contextAccessor;
        }

        [ThreadStatic]
        private static TSiteContext _context;

        /// <summary>
        /// 获取当前网站上下文。
        /// </summary>
        public TSiteContext SiteContext => _context ?? CreateSiteContext();

        /// <summary>
        /// 设置当前上下文实例。
        /// </summary>
        /// <param name="domain">域名地址，如果为空则从HTTP上下文中得到。</param>
        /// <returns>返回当前网站上下文实例。</returns>
        SiteContextBase ISiteContextAccessorBase.CreateSiteContext(string domain) => CreateSiteContext(domain);

        /// <summary>
        /// 通过域名获取当前上下文实例。
        /// </summary>
        /// <param name="domain">域名地址，如果为空则从HTTP上下文中得到。</param>
        /// <returns>返回当前网站上下文实例。</returns>
        public TSiteContext CreateSiteContext(string domain = null)
        {
            if (_context?.Initialized == true)
                throw new Exception(Resources.SiteContextIsInitialized);
            _context = new TSiteContext();
            if (domain == null)
                domain = _contextAccessor.HttpContext.Request.Host.Host;
            var siteDomain = _siteManager.GetDomain(domain);
            if (siteDomain != null)
            {
                _context.IsDefault = siteDomain.IsDefault;
                _context.Disabled = siteDomain.Disabled;
                _context.Domain = siteDomain.Domain;
                _context.Site = _siteManager.GetSite<TSite>(siteDomain.SiteId);
                _context.Initialized = true;
            }
            return _context;
        }

        /// <summary>
        /// 获取当前网站上下文。
        /// </summary>
        SiteContextBase ISiteContextAccessorBase.SiteContext => SiteContext;
    }
}