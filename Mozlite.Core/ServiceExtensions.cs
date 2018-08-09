using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        /// <returns>返回服务集合实例对象。</returns>
        public static IMozliteBuilder AddMozlite(this IServiceCollection services)
        {
            services.TryAddSingleton(typeof(IServiceAccessor<>), typeof(ServiceAccessor<>));
            var exportedTypes = GetExportedTypes();
            TryAddContainer(services, exportedTypes);
            return new MozliteBuilder(services);
        }

        private static void TryAddContainer(IServiceCollection services, IEnumerable<Type> exportedTypes)
        {
            foreach (var source in exportedTypes)
            {
                if (typeof(IServiceConfigurer).IsAssignableFrom(source))
                {
                    var service = Activator.CreateInstance(source) as IServiceConfigurer;
                    service?.ConfigureServices(services);
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

        private static IEnumerable<Type> GetExportedTypes()
        {
            var types = GetServices().ToList();
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

        private static IEnumerable<Type> GetServices()
        {
            var types = GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .ToList();
            foreach (var type in types)
            {
                var info = type.GetTypeInfo();
                if (info.IsPublic && info.IsClass && !info.IsAbstract && typeof(IService).IsAssignableFrom(type))
                    yield return type;
            }
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            foreach (var library in DependencyContext.Default.RuntimeLibraries)
            {
                if (library.Serviceable)
                    continue;
                assemblies.Add(Assembly.Load(new AssemblyName(library.Name)));
            }
            return assemblies;
        }
    }
}