using Microsoft.Extensions.DependencyInjection;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 服务扩展类。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 添加服务。
        /// </summary>
        /// <typeparam name="TSiteContextAccessor">访问器实现类。</typeparam>
        /// <param name="services">服务集合实例。</param>
        /// <returns>返回服务集合。</returns>
        public static IServiceCollection AddSitables<TSiteContextAccessor>(this IServiceCollection services)
            where TSiteContextAccessor : class, ISiteContextAccessorBase, new()
        {
            return services.AddSingleton<ISiteContextAccessorBase, TSiteContextAccessor>();
        }
    }
}