using Microsoft.AspNetCore.Http;
using Mozlite.Extensions.Installers;
using Mozlite.Mvc;
using System;

namespace Mozlite.Extensions.Extensions
{
    /// <summary>
    /// 实现当前网站上下文访问实例。
    /// </summary>
    /// <typeparam name="TSite">网站类型。</typeparam>
    /// <typeparam name="TSiteContext">网站上下文。</typeparam>
    public abstract class SiteContextAccessor<TSite, TSiteContext> : ISiteContextAccessor<TSite, TSiteContext>
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
        protected SiteContextAccessor(ISiteManager siteManager, IHttpContextAccessor contextAccessor)
        {
            _siteManager = siteManager;
            _contextAccessor = contextAccessor;
        }

        [ThreadStatic]
        private static TSiteContext _context;

        /// <summary>
        /// 获取当前网站上下文。
        /// </summary>
        public TSiteContext SiteContext => GetSiteContext(_contextAccessor.HttpContext) ?? GetThreadSiteContext();

        /// <summary>
        /// 设置当前上下文实例。
        /// </summary>
        /// <param name="siteKey">网站唯一键。</param>
        /// <returns>返回当前网站上下文实例。</returns>
        SiteContextBase ISiteContextAccessorBase.GetThreadSiteContext(string siteKey) => GetThreadSiteContext(siteKey);

        /// <summary>
        /// 设置当前上下文实例，后台现场中使用。
        /// </summary>
        /// <param name="siteKey">网站唯一键。</param>
        /// <returns>返回当前网站上下文实例。</returns>
        public TSiteContext GetThreadSiteContext(string siteKey = null)
        {
            if (_context != null)
                return _context;
            _context = new TSiteContext();
            if (InstallerHostedService.Current != InstallerStatus.Success) return _context;
            _context.Site = _siteManager.GetSiteByKey<TSite>(siteKey);
            _context.Domain = _siteManager.GetDomain(_context.SiteId);
            return _context;
        }

        private TSiteContext GetSiteContext(HttpContext context)
        {
            return context.GetOrCreate(typeof(SiteContextBase), () =>
              {
                  var domain = context.Request.GetDomain();
                  var siteDomain = _siteManager.GetDomain(domain);
                  if (siteDomain == null || siteDomain.Disabled)
                      return null;
                  var site = _siteManager.GetSite<TSite>(siteDomain.SiteId);
                  if (site == null)
                      return null;
                  return new TSiteContext { Domain = siteDomain, Site = site };
              });
        }

        /// <summary>
        /// 获取当前网站上下文。
        /// </summary>
        SiteContextBase ISiteContextAccessorBase.SiteContext => SiteContext;
    }
}