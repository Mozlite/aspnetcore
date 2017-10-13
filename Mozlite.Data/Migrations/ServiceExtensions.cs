using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite.Data.Migrations
{
    /// <summary>
    /// 数据库相关的服务器扩展。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 执行数据迁移。
        /// </summary>
        /// <param name="app">应用程序构建实例接口。</param>
        /// <returns>返回当前应用程序构建实例接口。</returns>
        public static IApplicationBuilder UseMigrations( this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                Task.Run(
                    async () => await serviceScope.ServiceProvider
                    .GetService<IDataMigrator>()
                    .MigrateAsync());
            }
            return app;
        }
    }
}