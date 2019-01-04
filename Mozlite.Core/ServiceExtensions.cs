using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Mozlite.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mozlite
{
    /// <summary>
    /// 服务扩展类。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 添加框架容器注册。
        /// </summary>
        /// <param name="services">服务容器集合。</param>
        /// <param name="configuration">配置接口。</param>
        /// <returns>返回服务集合实例对象。</returns>
        public static IMozliteBuilder AddMozlite(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton(typeof(IServiceAccessor<>), typeof(ServiceAccessor<>));
            var exportedTypes = GetExportedTypes(configuration);
            TryAddContainer(services, exportedTypes, configuration);
            return new MozliteBuilder(services, configuration);
        }

        private static void TryAddContainer(IServiceCollection services, IEnumerable<Type> exportedTypes, IConfiguration configuration)
        {
            foreach (var source in exportedTypes)
            {
                if (typeof(IServiceConfigurer).IsAssignableFrom(source))
                {
                    var service = Activator.CreateInstance(source) as IServiceConfigurer;
                    service?.ConfigureServices(services, configuration);
                }
                else if (typeof(IHostedService).IsAssignableFrom(source))
                {//后台任务
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), source));
                }
                else//注册类型
                {
                    var interfaceTypes = source.GetInterfaces()
                        .Where(itf => typeof(IService).IsAssignableFrom(itf));
                    foreach (var interfaceType in interfaceTypes)
                    {
                        if (typeof(ISingletonService).IsAssignableFrom(interfaceType))
                        {
                            services.TryAddSingleton(interfaceType, source);
                        }
                        else if (typeof(IScopedService).IsAssignableFrom(interfaceType))
                        {
                            services.TryAddScoped(interfaceType, source);
                        }
                        else if (typeof(ISingletonServices).IsAssignableFrom(interfaceType))
                        {
                            services.TryAddEnumerable(ServiceDescriptor.Singleton(interfaceType, source));
                        }
                        else if (typeof(IScopedServices).IsAssignableFrom(interfaceType))
                        {
                            services.TryAddEnumerable(ServiceDescriptor.Scoped(interfaceType, source));
                        }
                        else if (typeof(IServices).IsAssignableFrom(interfaceType))
                        {
                            services.TryAddEnumerable(ServiceDescriptor.Transient(interfaceType, source));
                        }
                        else
                        {
                            services.TryAddTransient(interfaceType, source);
                        }
                    }
                }
            }
        }

        private static IEnumerable<Type> GetExportedTypes(IConfiguration configuration)
        {
            var types = GetServices(configuration).ToList();
            var susppendServices = types.Select(type => type.GetTypeInfo())
                .Where(type => type.IsDefined(typeof(SuppressAttribute)))
                .ToList();
            var susppendTypes = new List<string>();
            foreach (var susppendService in susppendServices)
            {
                var suppendAttribute = susppendService.GetCustomAttribute<SuppressAttribute>();
                susppendTypes.Add(suppendAttribute.FullName);
            }
            susppendTypes = susppendTypes.Distinct().ToList();
            return types.Where(type => !susppendTypes.Contains(type.FullName))
                .ToList();
        }

        private static IEnumerable<Type> GetServices(IConfiguration configuration)
        {
            var types = GetAssemblies(configuration)
                .SelectMany(assembly => assembly.GetTypes())
                .ToList();
            foreach (var type in types)
            {
                var info = type.GetTypeInfo();
                if (info.IsPublic && info.IsClass && !info.IsAbstract && typeof(IService).IsAssignableFrom(type))
                    yield return type;
            }
        }

        private static IEnumerable<string> GetExcludeAssemblies(IConfiguration configuration)
        {
            return configuration.GetSection("Excludes").AsList() ?? Enumerable.Empty<string>();
        }

        private static IEnumerable<Assembly> GetAssemblies(IConfiguration configuration)
        {
            var assemblies = new List<Assembly>();
            var excludes = GetExcludeAssemblies(configuration);
            foreach (var library in DependencyContext.Default.RuntimeLibraries)
            {
                if (library.Serviceable || excludes.Contains(library.Name, StringComparer.OrdinalIgnoreCase))
                    continue;
                assemblies.Add(Assembly.Load(new AssemblyName(library.Name)));
            }
            return assemblies;
        }

        /// <summary>
        /// 使用配置。
        /// </summary>
        /// <param name="app">应用程序构建实例接口。</param>
        /// <param name="configuration">配置实例接口。</param>
        /// <returns>应用程序构建实例接口。</returns>
        public static IApplicationBuilder UseMozlite(this IApplicationBuilder app, IConfiguration configuration)
        {
            var services = app.ApplicationServices.GetService<IEnumerable<IApplicationConfigurer>>()
                .OrderByDescending(x => x.Priority)
                .ToArray();
            foreach (var service in services)
                service.Configure(app, configuration);
            return app;
        }

        /// <summary>
        /// 获取配置节点的字符串列表。
        /// </summary>
        /// <param name="section">配置节点。</param>
        /// <returns>返回当前配置的字符串列表。</returns>
        public static List<string> AsList(this IConfigurationSection section)
        {
            return section?.AsEnumerable().Where(x => x.Value != null).Select(x => x.Value).ToList();
        }
    }
}