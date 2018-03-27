using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Installers;
using Mozlite.Extensions.Tasks;
using Mozlite.Mvc;

namespace Mozlite.Extensions.Extensions
{
    /// <summary>
    /// 网站中间件。
    /// </summary>
    /// <typeparam name="TSiteContext">网站上下文。</typeparam>
    /// <typeparam name="TSite">网站实例类型。</typeparam>
    public class SiteMiddleware<TSiteContext, TSite>
        where TSite : SiteBase, new()
        where TSiteContext : SiteContextBase<TSite>, new()
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TSite> _logger;
        private readonly ISiteManager _siteManager;
        private readonly IInstallerManager _installerManager;

        /// <summary>
        /// 初始化类<see cref="SiteMiddleware{TSiteContext, TSite}"/>。
        /// </summary>
        /// <param name="next">下一个请求代理。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="siteManager">网站管理接口。</param>
        /// <param name="installerManager">安装管理接口。</param>
        public SiteMiddleware(RequestDelegate next, ILogger<TSite> logger, ISiteManager siteManager, IInstallerManager installerManager)
        {
            _next = next;
            _logger = logger;
            _siteManager = siteManager;
            _installerManager = installerManager;
        }

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="context">HTTP上下文。</param>
        /// <returns>返回当前任务。</returns>
        public async Task Invoke(HttpContext context)
        {
            await TaskHelper.WaitInitializingAsync();
            var path = context.Request.Path.Value.ToLower();
            if (IsIgnoredFilter(path))
            {
                await _next(context);
                return;
            }
            var site = await GetSiteContextAsync(context);
            if (site == null)
            {
                //新站安装请求
                var administrator = await _siteManager.GetAdministratorAsync<TSite>();
                if (!administrator.IsInitialized)
                {
                    context.Response.Redirect(InstallerPath);
                    return;
                }
                _logger.LogWarning("非法请求:{0}", context.Request.GetDisplayUrl());
                context.Response.StatusCode = 400;//BadRequest
                return;
            }
            await _next(context);
        }

        private async Task<TSiteContext> GetSiteContextAsync(HttpContext context)
        {
            var domain = context.Request.GetDomain();
            var siteDomain = await _siteManager.GetDomainAsync(domain);
            if (siteDomain == null || siteDomain.Disabled)
                return null;
            var site = await _siteManager.GetSiteAsync<TSite>(domain);
            if (site == null)
                return null;
            return Cache(context, new TSiteContext
            {
                Site = site,
                Domain = siteDomain
            });
        }

        private TSiteContext Cache(HttpContext context, TSiteContext siteContext)
        {
            context.Items[typeof(SiteContextBase)] = siteContext;
            return siteContext;
        }

        private static readonly string[] _filters =
        {
            "/installer/",
            "/dist/",
            "/images/",
            "/js/",
            "/css/",
            "/favicon.ico/"
        };

        private const string InstallerPath = "/installer";
        private static bool IsIgnoredFilter(string path)
        {
            path += '/';
            foreach (var filter in _filters)
            {
                if (path.StartsWith(filter))
                    return true;
            }
            return false;
        }
    }
}