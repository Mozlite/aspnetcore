using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 服务扩展类。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 添加服务。
        /// </summary>
        /// <typeparam name="TSiteContextAccessor">访问器实现类。</typeparam>
        /// <param name="services">服务集合实例。</param>
        /// <returns>返回服务集合。</returns>
        public static IServiceCollection AddSitables<TSiteContextAccessor>(this IServiceCollection services)
            where TSiteContextAccessor : class, ISiteContextAccessorBase, new()
        {
            return services.AddSingleton<ISiteContextAccessorBase, TSiteContextAccessor>();
        }

        public static IApplicationBuilder UseSitable(this IApplicationBuilder app)
        {
            return app.Use((context, next) =>
            {
                var siteContextAccessor = context.RequestServices.GetRequiredService<ISiteContextAccessorBase>();
                if (siteContextAccessor.SiteContext != null)
                    return next();
                return null;
            });
        }
    }
}