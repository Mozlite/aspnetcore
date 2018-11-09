using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Mozlite.Core.Tests
{
    /// <summary>
    /// 测试类。
    /// </summary>
    public static class Tests
    {
        static Tests()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _serviceProvider = Cores.BuildServiceProvider(configuration, current => current.AddServices(services =>
              {
                  services.TryAddSingleton(typeof(IConfiguration), x => configuration);

                  services.TryAddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
                  services.TryAddSingleton(typeof(ILoggerFactory), typeof(NullLoggerFactory));
              }));
        }

        private static readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 获取服务接口。
        /// </summary>
        /// <typeparam name="TService">服务接口类型。</typeparam>
        /// <returns>返回服务实例对象。</returns>
        public static TService GetService<TService>() => _serviceProvider.GetService<TService>();

        /// <summary>
        /// 获取服务接口。
        /// </summary>
        /// <typeparam name="TService">服务接口类型。</typeparam>
        /// <returns>返回服务实例对象。</returns>
        public static TService GetRequiredService<TService>() => _serviceProvider.GetRequiredService<TService>();
    }
}