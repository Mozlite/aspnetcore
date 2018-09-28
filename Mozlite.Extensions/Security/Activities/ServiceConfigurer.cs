using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 服务注册。
    /// </summary>
    /// <typeparam name="TActivityManager">用户活动管理接口。</typeparam>
    /// <typeparam name="TActivityManagerImplementation">实现类。</typeparam>
    /// <typeparam name="TUserActivity">用户活动类型。</typeparam>
    public abstract class ServiceConfigurer<TActivityManager, TActivityManagerImplementation, TUserActivity> : IServiceConfigurer
        where TActivityManager : IActivityManager<TUserActivity>
        where TActivityManagerImplementation : ActivityManager<TUserActivity>, TActivityManager
        where TUserActivity : UserActivity, new()
    {
        /// <summary>
        /// 配置服务方法。
        /// </summary>
        /// <param name="services">服务集合实例。</param>
        /// <param name="configuration">配置接口。</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IActivityManagerBase, TActivityManagerImplementation>();
            services.AddSingleton(typeof(TActivityManager), typeof(TActivityManagerImplementation));
        }
    }
}