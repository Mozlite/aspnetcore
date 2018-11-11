using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mozlite
{
    internal class MozliteBuilder : IMozliteBuilder
    {
        private readonly IServiceCollection _services;

        public MozliteBuilder(IServiceCollection services, IConfiguration configuration)
        {
            _services = services;
            Configuration = configuration;
        }

        /// <summary>
        /// 配置接口。
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 添加服务。
        /// </summary>
        /// <param name="action">配置服务代理类。</param>
        /// <returns>返回构建实例。</returns>
        public IMozliteBuilder AddServices(Action<IServiceCollection> action)
        {
            action(_services);
            return this;
        }
    }
}