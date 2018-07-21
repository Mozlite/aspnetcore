using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Mozlite.Mvc;

namespace Demo.Extensions.Security.Services
{
    /// <summary>
    /// 应用程序启动配置类。
    /// </summary>
    public class AuthenticationConfigurer : IApplicationConfigurer
    {
        /// <summary>
        /// 配置中间件使用模型。
        /// </summary>
        /// <param name="app">应用程序构建接口。</param>
        /// <param name="configuration">配置实例。</param>
        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseAuthentication();
        }
    }
}