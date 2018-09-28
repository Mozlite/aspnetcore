using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 注册验证处理类。
    /// </summary>
    public class PermissionAuthorizationConfigurer : IServiceConfigurer
    {
        /// <summary>
        /// 配置服务方法。
        /// </summary>
        /// <param name="services">服务集合实例。</param>
        /// <param name="configuration">配置接口。</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }
    }
}