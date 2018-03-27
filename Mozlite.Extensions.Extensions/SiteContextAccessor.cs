using System;
using Microsoft.AspNetCore.Http;
using Mozlite.Extensions.Extensions.Properties;
using Mozlite.Extensions.Installers;

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
        public TSiteContext SiteContext => _context ?? CreateSiteContext();

        /// <summary>
        /// 设置当前上下文实例。
        /// </summary>
        /// <param name="siteKey">网站唯一键。</param>
        /// <returns>返回当前网站上下文实例。</returns>
        SiteContextBase ISiteContextAccessorBase.CreateSiteContext(string siteKey) => CreateSiteContext(siteKey);

        /// <summary>
        /// 通过域名获取当前上下文实例。
        /// </summary>
        /// <param name="siteKey">网站唯一键。</param>
        /// <returns>返回当前网站上下文实例。</returns>
        public TSiteContext CreateSiteContext(string siteKey = null)
        {
            if (_context != null)
                throw new Exception(Resources.SiteContextIsInitialized);
            if (_contextAccessor.HttpContext?.Items[typeof(SiteContextBase)] is TSiteContext siteContext)
                return siteContext;
            _context = new TSiteContext();
            if (Installer.Current != InstallerStatus.Success) return _context;
            _context.Site = _siteManager.GetSiteByKey<TSite>(siteKey);
            _context.Domain = _siteManager.GetDomain(_context.SiteId);
            return _context;
        }

        /// <summary>
        /// 获取当前网站上下文。
        /// </summary>
        SiteContextBase ISiteContextAccessorBase.SiteContext => SiteContext;
    }
}