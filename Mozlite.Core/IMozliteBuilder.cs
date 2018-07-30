using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite
{
    /// <summary>
    /// Mozlite容器构建实例。
    /// </summary>
    public interface IMozliteBuilder
    {
        /// <summary>
        /// 添加服务。
        /// </summary>
        /// <param name="action">配置服务代理类。</param>
        /// <returns>返回构建实例。</returns>
        IMozliteBuilder AddServices(Action<IServiceCollection> action);
    }
}