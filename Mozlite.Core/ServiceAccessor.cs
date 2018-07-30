using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite
{
    /// <summary>
    /// 服务接口，可以访问更高等级得服务。
    /// </summary>
    /// <typeparam name="TService">当前服务接口。</typeparam>
    internal class ServiceAccessor<TService> : IServiceAccessor<TService>
    {
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 初始化类<see cref="ServiceAccessor{TService}"/>。
        /// </summary>
        /// <param name="serviceProvider">服务提供者接口。</param>
        public ServiceAccessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 接口实例对象。
        /// </summary>
        public TService Service => _serviceProvider.GetRequiredService<TService>();
    }
}