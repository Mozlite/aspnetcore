using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 服务扩展类。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 使用用户状态日志记录。
        /// </summary>
        /// <param name="app">应用程序构建实例。</param>
        /// <returns>返回应用程序构建实例。</returns>
        public static IApplicationBuilder UseUserActivity(this IApplicationBuilder app)
        {
            var factory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            factory.AddProvider(new ActivityLoggerProvider(app.ApplicationServices));
            return app;
        }
    }
}