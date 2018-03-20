using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Installers;

namespace Mozlite.Extensions.Sites
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
            if (Installer.Current == InstallerStatus.Success)
            {
                var uri = new Uri(context.Request.GetDisplayUrl());
                var domain = uri.DnsSafeHost;
                if (!uri.IsDefaultPort)
                    domain += ":" + uri.Port;
                var siteDomain = await _siteManager.GetDomainAsync(domain);
                if (siteDomain == null || siteDomain.Disabled)
                {
                    _logger.LogInformation("域名不存在或被禁用！");
                    context.Response.StatusCode = 400;//Bad Request
                    return;
                }
                var site = await _siteManager.GetSiteAsync<TSite>(domain);
                if (site == null)
                {
                    _logger.LogInformation("网站配置不存在！");
                    context.Response.StatusCode = 400;//Bad Request
                    return;
                }
                context.Items[typeof(SiteContextBase)] = new TSiteContext
                {
                    Site = site,
                    Domain = siteDomain
                };
            }
            else if (await IsInstalling(context))
            {
                context.Response.Redirect(InstallerPath);
                return;
            }
            await _next.Invoke(context);
        }

        private static readonly string[] _filters = new[]
        {
            "/installer",
            "/dist/",
            "/images/",
            "/js/",
            "/css/"
        };
        private const string InstallerPath = "/installer";
        private async Task<bool> IsInstalling(HttpContext context)
        {
            var path = context.Request.Path;
            foreach (var filter in _filters)
            {
                if (path.StartsWithSegments(filter, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return await _installerManager.IsNewAsync();
        }


    }
}