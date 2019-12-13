using Mozlite;
using Mozlite.Mvc;

namespace MozliteDemo.Extensions.Security
{
    /// <summary>
    /// 服务配置。
    /// </summary>
    public class ServiceConfigurer : IServiceConfigurer
    {
        /// <summary>
        /// 配置服务方法。
        /// </summary>
        /// <param name="builder">容器构建实例。</param>
        public void ConfigureServices(IMozliteBuilder builder)
        {
            builder.AddResources<ServiceConfigurer>();
        }
    }
}