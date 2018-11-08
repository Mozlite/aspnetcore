using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mozlite.Mvc.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mozlite.Mvc
{
    /// <summary>
    /// 服务扩展类。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 添加MVC服务。
        /// </summary>
        /// <param name="services">当前服务集合。</param>
        /// <returns>返回MVC构建实例。</returns>
        public static IMvcBuilder AddMozliteMvc(this IServiceCollection services)
        {//防止添加用户活动日志的时候未注册HTTP上下文实例。
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services.AddMvc()
                .AddRazorOptions(options =>
                {//网站的程序集名称，约定扩展程序集名称必须为“网站程序集名称.Extensions.当前扩展区域名称”
                    var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
                    ViewLocation(options, assemblyName);
                    PageLocation(options, assemblyName);
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        private static void PageLocation(RazorViewEngineOptions options, string assemblyName)
        {
            options.PageViewLocationFormats.Clear();
            options.AreaPageViewLocationFormats.Clear();
            options.PageViewLocationFormats.Add("/Pages/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.PageViewLocationFormats.Add("/Pages/Shared/{0}" + RazorViewEngine.ViewExtension);
            //区域试图路径：Extensions下面的路径，可以将每个区域改为扩展，这样会降低程序集的耦合度
            options.AreaPageViewLocationFormats.Add("/Extensions/" + assemblyName +
                                                ".Extensions.{2}/Pages/{1}/{0}" +
                                                RazorViewEngine.ViewExtension);
            options.AreaPageViewLocationFormats.Add("/Extensions/" + assemblyName +
                                                ".Extensions.{2}/Pages/Shared/{0}" +
                                                RazorViewEngine.ViewExtension);
            options.AreaPageViewLocationFormats.Add("/Extensions/{2}/Pages/{1}/{0}" +
                                                RazorViewEngine.ViewExtension);
            options.AreaPageViewLocationFormats.Add("/Extensions/{2}/Pages/Shared/{0}" +
                                                RazorViewEngine.ViewExtension);
            options.AreaPageViewLocationFormats.Add("/Pages/Shared/{0}" + RazorViewEngine.ViewExtension);
        }

        private static void ViewLocation(RazorViewEngineOptions options, string assemblyName)
        {
            options.ViewLocationFormats.Clear();
            options.AreaViewLocationFormats.Clear();
            options.ViewLocationFormats.Add("/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.ViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
            //区域试图路径：Extensions下面的路径，可以将每个区域改为扩展，这样会降低程序集的耦合度
            options.AreaViewLocationFormats.Add("/Extensions/" + assemblyName +
                                                ".Extensions.{2}/Views/{1}/{0}" +
                                                RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Extensions/" + assemblyName +
                                                ".Extensions.{2}/Views/Shared/{0}" +
                                                RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Extensions/{2}/Views/{1}/{0}" +
                                                RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Extensions/{2}/Views/Shared/{0}" +
                                                RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
        }

        /// <summary>
        /// 添加默认Mvc。
        /// </summary>
        /// <param name="app">APP构建实例对象。</param>
        /// <param name="configuration">配置实例对象。</param>
        /// <returns>APP构建实例对象。</returns>
        public static IApplicationBuilder UseMozliteMvc(this IApplicationBuilder app, IConfiguration configuration)
        {
            //配置程序集
            var services = app.ApplicationServices.GetService<IEnumerable<IApplicationConfigurer>>()
                .OrderByDescending(x => x.Priority)
                .ToArray();
            foreach (var service in services)
                service.Configure(app, configuration);
            //MVC
            app.UseMvc(builder =>
                builder
                    .MapLowerCaseRoute("dashboard-area-default", RouteSettings.Dashboard + "/{area}/{controller:regex(^admin.*)=Admin}/{action=Index}/{id?}")
                    .MapLowerCaseRoute("dashboard-default", RouteSettings.Dashboard + "/{controller:regex(^admin.*)=Admin}/{action=Index}/{id?}")
                    .MapLowerCaseRoute("area-default", "{area:exists}/{controller}/{action=Index}/{id?}")
                    .MapLowerCaseRoute("default", "{controller=Home}/{action=Index}/{id?}"));
            return app;
        }
    }
}