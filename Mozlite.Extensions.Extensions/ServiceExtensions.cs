using Mozlite.Extensions.Sites;
using Microsoft.AspNetCore.Builder;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 服务扩展类。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 检测并配置当前网站实例。
        /// </summary>
        /// <typeparam name="TSite">网站类型。</typeparam>
        /// <typeparam name="TSiteContext">网站上下文。</typeparam>
        /// <param name="app">应用程序构建实例。</param>
        /// <returns>返回应用程序构建实例。</returns>
        public static IApplicationBuilder UseSitable<TSiteContext, TSite>(this IApplicationBuilder app)
            where TSite : SiteBase, new()
            where TSiteContext : SiteContextBase<TSite>, new()
        {
            return app.UseMiddleware<SiteMiddleware<TSiteContext, TSite>>();
        }
    }
}