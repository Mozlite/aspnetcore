using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Extensions.Sites;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 服务扩展类。
    /// </summary>
    public static class ServiceExtensions
    {
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