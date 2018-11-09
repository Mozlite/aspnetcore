using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API扩张类。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 添加API服务描述。
        /// </summary>
        /// <param name="builder">容器构建实例。</param>
        /// <returns>返回服务集合实例。</returns>
        public static IMvcBuilder AddApis(this IMvcBuilder builder)
        {
            var services = builder.Services;
            services.TryAddSingleton<IApiDescriptionGroupCollectionProvider, ApiDescriptionGroupCollectionProvider>();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, DefaultApiDescriptionProvider>());
            return builder;
        }
    }
}