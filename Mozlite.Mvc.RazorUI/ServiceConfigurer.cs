using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite.Mvc.RazorUI
{
    /// <summary>
    /// 服务配置。
    /// </summary>
    public class ServiceConfigurer : IServiceConfigurer
    {
        /// <summary>
        /// 配置服务方法。
        /// </summary>
        /// <param name="services">服务集合实例。</param>
        /// <param name="configuration">配置接口。</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddResources<ServiceConfigurer>();
        }
    }
}