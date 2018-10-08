using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Mozlite.Extensions.Security.Activities;
using Mozlite.Mvc;

namespace MS.Extensions.Security.Activities
{
    /// <summary>
    /// 配置使用用户活动日志。
    /// </summary>
    public class ApplicationConfigurer : IApplicationConfigurer
    {
        /// <summary>
        /// 优先级。
        /// </summary>
        public int Priority => 0;

        /// <summary>
        /// 配置应用程序实例。
        /// </summary>
        /// <param name="app">应用程序构建实例。</param>
        /// <param name="configuration">配置接口。</param>
        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseUserActivity();
        }
    }
}