using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mozlite.Tasks
{
    /// <summary>
    /// 后台服务注册器。
    /// </summary>
    public class TaskServiceConfigurer : IServiceConfigurer
    {
        /// <summary>
        /// 配置服务方法。
        /// </summary>
        /// <param name="services">服务集合实例。</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHostedService, TaskHostedService>();
        }
    }
}