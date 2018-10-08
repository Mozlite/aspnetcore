using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Mozlite.Mvc;

namespace MS.Extensions.Security.Services
{
    /// <summary>
    /// 应用程序启动配置类。
    /// </summary>
    public class ApplicationConfigurer : IApplicationConfigurer
    {
        /// <summary>
        /// 优先级。
        /// </summary>
        public int Priority => 1;

        /// <summary>
        /// 配置中间件使用模型。
        /// </summary>
        /// <param name="app">应用程序构建接口。</param>
        /// <param name="configuration">配置实例。</param>
        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseCookiePolicy();
            app.UseAuthentication();
        }
    }
}